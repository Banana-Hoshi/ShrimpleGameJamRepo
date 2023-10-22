using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerInputManager inputManager;
	public PizzaParlour parlour1;
	public PizzaParlour parlour2;
	public Camera cam;
	public LookatThing p1Arrow;
	public LookatThing p2Arrow;
	public List<StayInTriggerCheck> triggers;
	public int numberOfRounds = 5;
	StayInTriggerCheck current = null;


	int player1Layer;
    int player2Layer;
    int curLayer;

    private void Awake()
    {
        player1Layer = LayerMask.NameToLayer("Camera1");
        player2Layer = LayerMask.NameToLayer("Camera2");
        curLayer = player1Layer;
    }

    private void OnPlayerJoined(PlayerInput playerInput)
    {
		if (curLayer < 0)
			return;
		
		cam.gameObject.SetActive(false);

        GameObject player = playerInput.gameObject;
        foreach (var cam in player.GetComponentsInChildren<CinemachineVirtualCamera>(true))
        {
            cam.gameObject.layer = curLayer;
        }
        player.GetComponentInChildren<Camera>().gameObject.layer = curLayer;
        player.GetComponentInChildren<Camera>().cullingMask = player.GetComponentInChildren<Camera>().cullingMask | (1 << curLayer);

        if (curLayer == player1Layer) {
			parlour1.pachinko.bike = player.GetComponent<Bike>();
			parlour1.pachinko.bike.enabled = false;
			parlour1.pachinko.manager = player.GetComponent<PizzaManager>();
			player.transform.position = parlour1.spawnPoint.position;
			player.transform.rotation = parlour1.spawnPoint.rotation;

			player.GetComponent<Rigidbody>().isKinematic = true;

            curLayer = player2Layer;
        }
		else if (curLayer == player2Layer) {
			parlour2.pachinko.bike = player.GetComponent<Bike>();
			parlour2.pachinko.manager = player.GetComponent<PizzaManager>();
			player.transform.position = parlour2.spawnPoint.position;
			player.transform.rotation = parlour2.spawnPoint.rotation;

			parlour1.pachinko.bike.GetComponent<Rigidbody>().isKinematic = false;

			StartGame();

			curLayer = -1;
		}
    }

	int round;

	void StartGame() {
		parlour1.pachinko.bike.enabled = true;
		parlour2.pachinko.bike.enabled = true;

		round = 0;

		LoadRound();
	}

	public void LoadRound() {
		if (++round > numberOfRounds) {
			SceneManager.LoadScene(0);
		}

		if (current) {
			current.winner -= CheckWin;
		}

		current = triggers[Random.Range(0, triggers.Count)];

		current.winner += CheckWin;


	}

	public void Send(List<StayInTriggerCheck> trigger) {
		triggers = trigger;
	}

	public void CheckWin(GameObject winner) {
		PizzaManager manager = winner.GetComponent<PizzaManager>();
		if (manager && manager.ammo > 0) {
			if (parlour1.pachinko.manager == manager) {
				parlour1.AddScore(manager.ammo);
				manager.SetAmmo(0);
			}
			else {
				parlour2.AddScore(manager.ammo);
				manager.SetAmmo(0);
			}
			LoadRound();
		}
	}
}
