using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaParlour : MonoBehaviour
{
	public Pachinko pachinko;
	public Transform spawnPoint;
	StayInTriggerCheck door;
	public GameManager manager;

	public int score = 0;
	public int multiplier = 100;

	private void Awake() {
		door = GetComponentInChildren<StayInTriggerCheck>();
		door.winner += Check;
		pachinko.parlour = this;
	}

	void Check(GameObject user) {
		if (pachinko && user == pachinko.bike.gameObject && pachinko.manager.ammo < 16) {
			//start pachinko
			pachinko.StartMachine();
		}
	}

	public void AddScore(int amt) {
		//calculated based on current ammo
		score += amt * multiplier;
	}
}
