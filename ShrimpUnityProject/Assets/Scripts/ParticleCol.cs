using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCol : MonoBehaviour
{
	public Rigidbody holder;
	void OnParticleCollision(GameObject other) {
		if (other != holder.gameObject) {
			Debug.Log("hit!");
		}
	}
}
