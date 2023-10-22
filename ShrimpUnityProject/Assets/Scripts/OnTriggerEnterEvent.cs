using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerEnterEvent : MonoBehaviour
{
	public event System.Action<int> triggered;

	public int var;

	private void OnTriggerEnter(Collider other) {
		triggered?.Invoke(var);
	}
}
