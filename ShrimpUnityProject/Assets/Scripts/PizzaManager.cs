using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PizzaManager : MonoBehaviour
{
	public Bike bike;
	public float effectTime = 10f;
	public GameObject projectile;
	public ParticleSystem particles;
	public Material sharedSmoke;
	public Material sharedFire;
	void OnParticleCollision(GameObject other) {
		if (other != particles.gameObject) {
			
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

	private void Start() {
		
	}

	IEnumerator DelayedFunc(float time, System.Action action) {
		yield return new WaitForSeconds(time);
		action.Invoke();
	}
}
