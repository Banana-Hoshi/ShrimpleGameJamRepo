using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInTriggerCheck : MonoBehaviour
{
	Dictionary<string, float> objects = new Dictionary<string, float>();

	public event System.Action<GameObject> winner;
	public float winTime = 2f;
	public Renderer colourHint;
	public Color hintColour = Color.yellow;
	Color cachedColour;

	private void Awake() {
		if (colourHint) {
			cachedColour = colourHint.sharedMaterial.color;
		}
	}

	private void OnTriggerEnter(Collider other) {
		PizzaManager manager = other.attachedRigidbody?.GetComponent<PizzaManager>();
		if (manager) {
			if (!objects.ContainsKey(other.attachedRigidbody.name)) {
				objects.Add(other.attachedRigidbody.name, 0f);
			}
		}

		if (colourHint && objects.Count > 0) {
			colourHint.material.color = hintColour;
		}
	}

	private void OnTriggerStay(Collider other) {
		if (objects.ContainsKey(other.attachedRigidbody?.name)) {
			objects[other.attachedRigidbody.name] += Time.fixedDeltaTime;
			if (objects[other.attachedRigidbody.name] > winTime) {
				winner?.Invoke(other.attachedRigidbody.gameObject);
				objects.Remove(other.attachedRigidbody.name);
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.attachedRigidbody)
			objects.Remove(other.attachedRigidbody.name);

		if (colourHint && objects.Count == 0) {
			colourHint.material.color = cachedColour;
		}
	}

	private void OnDisable() {
		objects.Clear();
		if (colourHint) {
			colourHint.material.color = cachedColour;
		}
	}
}
