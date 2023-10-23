using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PizzaManager : MonoBehaviour
{
    public PizzaParlour parlour;
    public PlayerInput input;
    public Bike bike;
	public LookatThing arrow;
    public float shootDelay = 5f;
    public Transform shootPoint;
    public float effectTime = 10f;
    public PizzaProjectile projectile;
    public ParticleSystem particles;
    public Material sharedSmoke;
    public Material sharedFire;
    public List<Collider> tires;
    public int effect;
    public int ammo = 0;
    public Image[] ammoSprites;
    public Image topping;
    public GameObject regularCam, mushroomCam;
    public UniversalAdditionalCameraData cam;

    PhysicMaterial tireMat;
    float defaultFriction;
    float defaultFriction2;
    float delayTime = -1f;
    float defaultFix;

    private void Awake()
    {
        defaultFix = bike.fixForce;
    }

    /*
	void OnParticleCollision(GameObject other) {
		if (other != particles.gameObject) {
			
		}
	}*/

    public void Hit(int damage)
    {
        SetAmmo(ammo - damage);
    }

    void Shoot(InputAction.CallbackContext ctx)
    {
        //do thing
        if (ammo == 0 || delayTime > 0f)
            return;

        PizzaProjectile obj = Instantiate(projectile, shootPoint.position, shootPoint.rotation);

        obj.Shoot(this, transform.forward, effect);

        SetAmmo(ammo - 1);

        delayTime = shootDelay;
        StartCoroutine(DelayedFunc(shootDelay, Reload));
    }

    void Reload()
    {
        delayTime = 0f;
        //do other things
    }

	public void AddAmmo(int amt) {
		SetAmmo(Mathf.Min(ammo + amt, ammoSprites.Length));
	}

    public void SetAmmo(int amt)
    {
        int newAmmo = Mathf.Max(amt, 0);
        //do visual changes here
        if (newAmmo > ammo)
        {
            for (int i = ammo; i < newAmmo; ++i)
            {
                ammoSprites[i].enabled = true;
            }
        }
        else if (newAmmo < ammo)
        {
            for (int i = ammo - 1; i >= newAmmo; --i)
            {
                ammoSprites[i].enabled = false;
            }
        }
        ammo = newAmmo;
        bike.fixForce = defaultFix * (2f - (ammo / ammoSprites.Length)) * 0.5f;

		if (ammo == 0) {
			effect = 0;
			topping.sprite = null;
			topping.color = Color.clear;
			arrow.target = parlour.pachinko.transform;
		}
    }

    private void Start()
    {
        tireMat = tires[0].material;
        foreach (Collider col in tires)
        {
            col.sharedMaterial = tireMat;
        }
        defaultFriction = tireMat.dynamicFriction;
        defaultFriction2 = tireMat.staticFriction;



        int newAmmo = ammo;
        ammo = ammoSprites.Length;
        SetAmmo(newAmmo);
    }

    public void Effect(int effect)
    {
        switch (effect)
        {
            case 1:
                SetJalepenio();
                break;
            case 2:
                SetMushroom();
                break;
            case 3:
                SetFish();
                break;
        }
    }

    public void SetJalepenio()
    {
        particles.GetComponent<ParticleSystemRenderer>().sharedMaterial = sharedFire;
        ParticleSystem.MainModule main = particles.main;
        main.startLifetime = 2f;
		if (bike.jalepenioModeTimer <= 0f) {
			bike.jalepenioModeTimer = 1f;
	        StartCoroutine(DelayedFuncCondition(delegate () {return bike.jalepenioModeTimer <= 0f;}, RemoveJalepenio));
		}
        bike.SetJalepenio(effectTime);
    }

    public void RemoveJalepenio()
    {
        particles.GetComponent<ParticleSystemRenderer>().sharedMaterial = sharedSmoke;
        ParticleSystem.MainModule main = particles.main;
        main.startLifetime = 1f;
    }

    public void SetMushroom()
    {
        //do visual effects here
        mushroomCam.SetActive(true);
        cam.renderPostProcessing = true;

		if (bike.mushroomModeTimer <= 0f) {
			bike.mushroomModeTimer = 1f;
			StartCoroutine(DelayedFuncCondition(delegate () { return bike.mushroomModeTimer <= 0f; }, RemoveMushroom));
		}
		bike.SetMushroom(effectTime);
    }

    public void RemoveMushroom()
    {

        cam.renderPostProcessing = false;
        mushroomCam.SetActive(false);

    }

    public void SetFish()
    {
        tireMat.dynamicFriction = 0f;
        tireMat.staticFriction = 0f;
		
		if (bike.fishModeTimer <= 0f) {
			bike.fishModeTimer = 1f;
	        StartCoroutine(DelayedFuncCondition(delegate () {return bike.fishModeTimer <= 0f;}, RemoveFish));
		}
        bike.SetFish(effectTime);
    }

    public void RemoveFish()
    {
        tireMat.dynamicFriction = defaultFriction;
        tireMat.staticFriction = defaultFriction2;
    }

    IEnumerator DelayedFunc(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        action.Invoke();
    }

    IEnumerator DelayedFuncCondition(System.Func<bool> condition, System.Action action)
    {
        yield return new WaitUntil(condition);
        action.Invoke();
    }

    void OneHit()
    {
        Hit(1);
    }

    private void OnEnable()
    {
        InputAction shoot = input.currentActionMap.FindAction("Shoot");
        shoot.started += Shoot;
        bike.tooMuchTilt += OneHit;
    }

    private void OnDisable()
    {
        InputAction shoot = input.currentActionMap.FindAction("Shoot");
        shoot.started -= Shoot;
        bike.tooMuchTilt -= OneHit;
    }
}
