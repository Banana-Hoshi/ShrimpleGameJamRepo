using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PizzaManager : MonoBehaviour
{
	public PlayerInput input;
	public Bike bike;
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
	public Sprite toppings;

	PhysicMaterial tireMat;
	float defaultFriction;
	float defaultFriction2;
	float delayTime = -1f;
	float defaultFix;

	private void Awake() {
		defaultFix = bike.fixForce;
	}

	/*
	void OnParticleCollision(GameObject other) {
		if (other != particles.gameObject) {
			
		}
	}*/

	public void Hit(int damage) {
		SetAmmo(ammo - damage);
	}

	void Shoot(InputAction.CallbackContext ctx) {
		//do thing
		if (ammo == 0 || delayTime > 0f)
			return;

		PizzaProjectile obj = Instantiate(projectile, shootPoint.position, shootPoint.rotation);

		obj.Shoot(this, transform.forward, effect);

		SetAmmo(ammo - 1);

		delayTime = shootDelay;
		StartCoroutine(DelayedFunc(shootDelay, Reload));
	}

	void Reload() {
		delayTime = 0f;
		//do other things
	}

	public void SetAmmo(int amt) {
		int newAmmo = Mathf.Max(amt, 0);
		//do visual changes here
		if (newAmmo > ammo) {
			for (int i = ammo; i < newAmmo; ++i) {
				ammoSprites[i].enabled = true;
			}
		}
		else if (newAmmo < ammo) {
			for (int i = ammo - 1; i >= newAmmo; --i) {
				ammoSprites[i].enabled = false;
			}
		}
		ammo = newAmmo;
		bike.fixForce = defaultFix * (2f - (ammo / ammoSprites.Length)) * 0.5f;
	}

	private void Start() {
		tireMat = tires[0].material;
		foreach(Collider col in tires) {
			col.sharedMaterial = tireMat;
		}
		defaultFriction = tireMat.dynamicFriction;
		defaultFriction2 = tireMat.staticFriction;



		int newAmmo = ammo;
		ammo = ammoSprites.Length;
		SetAmmo(newAmmo);
	}

	public void Effect(int effect) {
		switch (effect) {
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

	public void SetJalepenio() {
		particles.GetComponent<ParticleSystemRenderer>().sharedMaterial = sharedFire;
		ParticleSystem.MainModule main = particles.main;
		main.startLifetime = 2f;
		bike.SetJalepenio(effectTime);
		StartCoroutine(DelayedFunc(effectTime, RemoveJalepenio));
	}

	public void RemoveJalepenio() {
		particles.GetComponent<ParticleSystemRenderer>().sharedMaterial = sharedSmoke;
		ParticleSystem.MainModule main = particles.main;
		main.startLifetime = 1f;
	}

	public void SetMushroom() {
		bike.SetMushroom(effectTime);
		//do visual effects here
	}

	public void SetFish() {
		tireMat.dynamicFriction = 0f;
		tireMat.staticFriction = 0f;
		bike.SetFish(effectTime);
		StartCoroutine(DelayedFunc(effectTime, RemoveFish));
	}

	public void RemoveFish() {
		tireMat.dynamicFriction = defaultFriction;
		tireMat.staticFriction = defaultFriction2;
	}

	IEnumerator DelayedFunc(float time, System.Action action) {
		yield return new WaitForSeconds(time);
		action.Invoke();
	}

	void OneHit() {
		Hit(1);
	}

	private void OnEnable() {
		InputAction shoot = input.currentActionMap.FindAction("Shoot");
		shoot.started += Shoot;
		bike.tooMuchTilt += OneHit;
	}

	private void OnDisable() {
		InputAction shoot = input.currentActionMap.FindAction("Shoot");
		shoot.started -= Shoot;
		bike.tooMuchTilt -= OneHit;
	}
}
