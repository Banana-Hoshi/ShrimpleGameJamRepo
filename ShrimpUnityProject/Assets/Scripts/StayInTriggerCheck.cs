using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInTriggerCheck : MonoBehaviour
{
	Dictionary<GameObject, float> objects = new Dictionary<GameObject, float>();

	public event System.Action<GameObject> winner;
	public float winTime = 2f;

	private void OnTriggerEnter(Collider other) {
		PizzaManager manager = other.attachedRigidbody?.GetComponent<PizzaManager>();
		if (manager) {
			if (!objects.ContainsKey(other.attachedRigidbody.gameObject)) {
				objects.Add(other.attachedRigidbody.gameObject, 0f);
			}
		}
	}

	private void OnTriggerStay(Collider other) {
		if (objects.ContainsKey(other.attachedRigidbody?.gameObject)) {
			objects[other.attachedRigidbody.gameObject] += Time.fixedDeltaTime;
			if (objects[other.attachedRigidbody.gameObject] > winTime) {
				winner?.Invoke(other.attachedRigidbody.gameObject);
				objects.Remove(other.attachedRigidbody.gameObject);
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.attachedRigidbody)
			objects.Remove(other.attachedRigidbody.gameObject);
	}

	private void OnDisable() {
		objects.Clear();
	}
}
