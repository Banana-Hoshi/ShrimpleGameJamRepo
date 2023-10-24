using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInTriggerCheck : MonoBehaviour
{
	public class Pair<T, T2> {
		public Pair(T first, T2 second) {
			this.first = first;
			this.second = second;
		}
		public T first;
		public T2 second;
	}
	Dictionary<Rigidbody, Pair<int, float>> objects = new Dictionary<Rigidbody, Pair<int, float>>();

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
			if (objects.ContainsKey(other.attachedRigidbody)) {
				objects[other.attachedRigidbody].first += 1;
			}
			else {
				objects.Add(other.attachedRigidbody, new Pair<int, float>(1, 0f));
			}
		}

		if (colourHint && objects.Count > 0) {
			colourHint.material.color = hintColour;
		}
	}

	private void OnTriggerStay(Collider other) {
		if (objects.ContainsKey(other.attachedRigidbody)) {
			objects[other.attachedRigidbody].second += Time.fixedDeltaTime;
			if (objects[other.attachedRigidbody].second > winTime) {
				winner?.Invoke(other.attachedRigidbody.gameObject);
				
				objects.Clear();

				if (colourHint) {
					colourHint.material.color = cachedColour;
				}
			}
		}
	}

	private void OnTriggerExit(Collider other) {
		if (objects.ContainsKey(other.attachedRigidbody)) {
			objects[other.attachedRigidbody].first -= 1;
			if (objects[other.attachedRigidbody].first == 0) {
				objects.Remove(other.attachedRigidbody);

				if (colourHint && objects.Count == 0) {
					colourHint.material.color = cachedColour;
				}
			}
		}

	}

	private void OnDisable() {
		objects.Clear();
		if (colourHint) {
			colourHint.material.color = cachedColour;
		}
	}
}
