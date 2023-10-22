using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pachinko : MonoBehaviour
{
	public PizzaManager manager;
	public Bike bike;
	public Sprite[] toppings;
	public Rigidbody pizza;
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

		//setup the dropper here
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

		manager.SetAmmo(8);
		CloseMachine();
	}

	public void CloseMachine() {
		foreach (OnTriggerEnterEvent evnt in jalepenioEvents)
			evnt.triggered -= Check;
		foreach (OnTriggerEnterEvent evnt in mushroomEvents)
			evnt.triggered -= Check;
		foreach (OnTriggerEnterEvent evnt in fishEvents)
			evnt.triggered -= Check;
	}

	IEnumerator PachinkoInput() {
		while (true) {
			

			yield return null;
		}
	}
}
