using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaBox : MonoBehaviour
{
	public Rigidbody rb;
	public Collider col;
	public HingeJoint lid;
	public Vector3 launch = Vector3.up * 10f;
	public float lifeSpan = 10f;
	public GameObject pizza;

	public void AddToBike() {
		rb.isKinematic = true;
		col.enabled = false;
	}

	public void CloseLid() {
		JointLimits limit = lid.limits;
		limit.max = -89f;
		lid.limits = limit;
		lid.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
	}

	public void OpenLid() {
		JointLimits limit = lid.limits;
		limit.max = 0f;
		lid.limits = limit;
		lid.transform.localRotation = Quaternion.identity;
	}

	public void Eject() {
		OpenLid();
		rb.isKinematic = false;
		col.enabled = true;
		transform.SetParent(null);
		rb.velocity = launch;
		pizza.SetActive(false);
		Destroy(gameObject, lifeSpan);
	}
}
