using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookatThing : MonoBehaviour
{
	public Transform target;
	[SerializeField] Vector3 axis = Vector3.forward;

	private void Update() {
		if (!target)
			return;
		
		transform.rotation = Quaternion.Euler(0f,
			Vector3.SignedAngle(axis, target.position - transform.position, Vector3.up),
			0f);
	}
}
