using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInTriggerCheck : MonoBehaviour
{
	Dictionary<GameObject, float> objects = new Dictionary<GameObject, float>();

	public event System.Action<GameObject> winner;
	public float winTime = 2f;

	private void OnTriggerEnter(Collider other) {
		PizzaManager manager = other.GetComponent<PizzaManager>();
		if (manager) {
			if (!objects.ContainsKey(other.gameObject)) {
				objects.Add(other.gameObject, 0f);
			}
		}
	}

	private void OnTriggerStay(Collider other) {
		if (objects.ContainsKey(other.gameObject)) {
			objects[other.gameObject] += Time.fixedDeltaTime;
			if (objects[other.gameObject] > winTime) {
				winner?.Invoke(other.gameObject);
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		objects.Remove(other.gameObject);
	}

	private void OnDisable() {
		objects.Clear();
	}
}
