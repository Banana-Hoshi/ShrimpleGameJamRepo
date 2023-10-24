using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PizzaProjectile : MonoBehaviour
{
	public Rigidbody bod;
	public float speed = 40f;
	public float bump = 5f;
	public Collider col;
	public Transform pizza;
	Vector3 angularVelo;
	int effect;
	PizzaManager owner;
	public void Shoot(PizzaManager own, Vector3 direction, int eft) {
		bod.velocity = direction * speed + Vector3.up * bump;
		angularVelo = new Vector3(
			Random.Range(-1f, 1f),
			Random.Range(-1f, 1f),
			Random.Range(-1f, 1f)
			).normalized * Random.Range(5f, 10f) * Time.fixedDeltaTime;
		effect = eft;
		owner = own;
		StartCoroutine(Die());
	}

	private void FixedUpdate() {
		pizza.Rotate(angularVelo);
	}

	private void OnCollisionEnter(Collision other) {
		PizzaManager manager = other.gameObject.GetComponent<PizzaManager>();
		if (manager && manager != owner) {
			manager.Effect(effect);
			col.enabled = false;
			bod.velocity = Vector3.zero;
			bod.useGravity = false;
		}
	}

	IEnumerator Die() {
		yield return new WaitForSeconds(5f);
		Destroy(gameObject);
	}
}
