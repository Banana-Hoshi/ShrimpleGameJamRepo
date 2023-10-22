using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PizzaParlour : MonoBehaviour
{
	public Pachinko pachinko;
	StayInTriggerCheck door;
	private void Awake() {
		door = GetComponentInChildren<StayInTriggerCheck>();
		door.winner += Check;
	}

	void Check(GameObject user) {
		if (pachinko && user == pachinko.bike.gameObject) {
			//start pachinko
			pachinko.StartMachine();
			pachinko.bike.enabled = false;
		}
	}
}