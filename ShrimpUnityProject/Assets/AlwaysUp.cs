using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlwaysUp : MonoBehaviour
{
	public Transform counter;
	private void FixedUpdate() {
		transform.localRotation = Quaternion.Euler(-counter.eulerAngles.x, 0f, 0f);
	}
}
