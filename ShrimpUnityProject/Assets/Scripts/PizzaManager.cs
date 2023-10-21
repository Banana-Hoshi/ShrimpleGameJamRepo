using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PizzaManager : MonoBehaviour
{
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
	}

	public void RemoveJalepenio() {
		particles.GetComponent<ParticleSystemRenderer>().sharedMaterial = sharedSmoke;
		ParticleSystem.MainModule main = particles.main;
		main.startLifetime = 1f;
	}

	private void Start() {
		if (Random.Range(0, 2) > 0) {
			SetJalepenio();
		}
	}
}
