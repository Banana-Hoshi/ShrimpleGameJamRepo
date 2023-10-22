using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pachinko : MonoBehaviour
{
	public PizzaParlour parlour;
	public PizzaManager manager;
	public Bike bike;
	public Sprite[] toppings;
	public Rigidbody pizza;
	public Transform left, right;
	public CinemachineVirtualCamera cam;
	//1
	public List<OnTriggerEnterEvent> jalepenioEvents;
	//2
	public List<OnTriggerEnterEvent> mushroomEvents;
	//3
	public List<OnTriggerEnterEvent> fishEvents;

	Rigidbody tempPizza;

	private void Awake() {
		int index = 1;
		foreach (OnTriggerEnterEvent evnt in jalepenioEvents)
			evnt.var = index;
		index = 2;
		foreach (OnTriggerEnterEvent evnt in mushroomEvents)
			evnt.var = index;
		index = 3;
		foreach (OnTriggerEnterEvent evnt in fishEvents)
			evnt.var = index;
	}

	public void StartMachine() {
		foreach (OnTriggerEnterEvent evnt in jalepenioEvents)
			evnt.triggered += Check;
		foreach (OnTriggerEnterEvent evnt in mushroomEvents)
			evnt.triggered += Check;
		foreach (OnTriggerEnterEvent evnt in fishEvents)
			evnt.triggered += Check;

		bike.enabled = false;
		bike.transform.position = parlour.transform.position;
		bike.rb.useGravity = false;
		bike.rb.velocity = Vector3.zero;

		cam.gameObject.SetActive(true);

		//setup the dropper here
		tempPizza = Instantiate(pizza, (left.position + right.position) / 2f, Quaternion.identity);
		tempPizza.transform.localScale = transform.localScale;
		tempPizza.transform.LookAt(cam.transform);
		tempPizza.isKinematic = true;

		StartCoroutine(PachinkoInput());
	}

	void Check(int var) {
		manager.effect = var;
		if (var > 0 && var < toppings.Length + 1) {
			manager.topping.sprite = toppings[var - 1];
			manager.topping.color = Color.white;
		}
		else {
			manager.topping.color = Color.clear;
		}

		manager.AddAmmo(8);
		CloseMachine();
	}

	public void CloseMachine() {
		foreach (OnTriggerEnterEvent evnt in jalepenioEvents)
			evnt.triggered -= Check;
		foreach (OnTriggerEnterEvent evnt in mushroomEvents)
			evnt.triggered -= Check;
		foreach (OnTriggerEnterEvent evnt in fishEvents)
			evnt.triggered -= Check;
		
		Destroy(tempPizza.gameObject);
		tempPizza = null;

		bike.enabled = true;
		bike.transform.position = parlour.spawnPoint.position;
		bike.rb.useGravity = true;

		cam.gameObject.SetActive(false);
	}

	IEnumerator PachinkoInput() {
		InputAction shoot = bike.inputMap.currentActionMap.FindAction("Shoot");
		Transform target = right;
		while (true) {
			tempPizza.position = Vector3.MoveTowards(tempPizza.position, target.position, Random.Range(0f, 5f) * Time.deltaTime);
			if (tempPizza.position == target.position) {
				target = target == right ? left : right;
			}

			if (shoot.triggered) {
				tempPizza.isKinematic = false;
				break;
			}
			yield return null;
		}
	}
}
