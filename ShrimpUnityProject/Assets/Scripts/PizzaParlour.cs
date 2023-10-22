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
	}

	void Check(GameObject user) {
		if (pachinko && user == pachinko.bike.gameObject && pachinko.manager.ammo < 16) {
			//start pachinko
			pachinko.StartMachine();
		}
	}

	public void AddScore(int useless = 0) {
		//calculate based on current ammo
		if (pachinko.manager.ammo > 0) {
			score += pachinko.manager.ammo * multiplier;
			pachinko.manager.SetAmmo(0);
			manager.LoadRound();
		}
	}
}
