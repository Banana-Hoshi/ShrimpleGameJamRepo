using System.Collections;
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
	public Transform houseEffect;
	public List<StayInTriggerCheck> triggers;
	public int numberOfRounds = 5;
	public GameObject overlayCanvas;
	public TMPro.TMP_Text roundText;
	public TMPro.TMP_Text winnerText;
	StayInTriggerCheck current = null;

	PlayerInput tempInput;

	int player1Layer;
    int player2Layer;
    int curLayer;

    private void Awake()
    {
        player1Layer = LayerMask.NameToLayer("Camera1");
        player2Layer = LayerMask.NameToLayer("Camera2");
        curLayer = player1Layer;
		roundText.text = "";
		overlayCanvas.SetActive(false);
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
			parlour1.pachinko.manager.parlour = parlour1;
			player.transform.position = parlour1.spawnPoint.position;
			player.transform.rotation = parlour1.spawnPoint.rotation;
			parlour1.text = player.GetComponentInChildren<TMPro.TMP_Text>();

			player.GetComponent<Rigidbody>().isKinematic = true;

            curLayer = player2Layer;
			StartCoroutine(AddSoloCheck(playerInput));
        }
		else if (curLayer == player2Layer) {
			parlour2.pachinko.bike = player.GetComponent<Bike>();
			parlour2.pachinko.bike.enabled = false;
			parlour2.pachinko.manager = player.GetComponent<PizzaManager>();
			parlour2.pachinko.manager.parlour = parlour2;
			player.transform.position = parlour2.spawnPoint.position;
			player.transform.rotation = parlour2.spawnPoint.rotation;
			parlour2.text = player.GetComponentInChildren<TMPro.TMP_Text>();

			player.GetComponent<Rigidbody>().isKinematic = true;

			StartCoroutine(StartGame());

			curLayer = -1;
			if (tempInput) {
				tempInput.currentActionMap.FindAction("Shoot").started -= StartSolo;
				tempInput = null;
			}
		}
    }

	int round;

	IEnumerator StartGame() {
		overlayCanvas.SetActive(true);
		for (int i = 5; i > 0; --i) {
			winnerText.text = i.ToString();
			yield return new WaitForSeconds(1f);
		}
		winnerText.text = "Go!";

		parlour1.pachinko.bike.enabled = true;
		if (parlour2.pachinko.bike)
			parlour2.pachinko.bike.enabled = true;

		round = 0;

		parlour1.pachinko.bike.GetComponent<Rigidbody>().isKinematic = false;
		if (parlour2.pachinko.bike)
			parlour2.pachinko.bike.GetComponent<Rigidbody>().isKinematic = false;

		LoadRound();

		yield return new WaitForSeconds(3f);
		winnerText.text = "";
	}

	public void LoadRound() {
		if (current) {
			current.winner -= CheckWin;
			current.enabled = false;
			current.GetComponent<Collider>().enabled = false;
		}

		if (++round > numberOfRounds) {
			StartCoroutine(WinnerCheck());
			return;
		}
		roundText.text = round.ToString();

		current = triggers[Random.Range(0, triggers.Count)];

		current.winner += CheckWin;
		current.enabled = true;
		current.GetComponent<Collider>().enabled = true;

		houseEffect.position = current.transform.position;
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

	IEnumerator AddSoloCheck(PlayerInput playerInput) {
		yield return new WaitForSeconds(1f);
		if (parlour2.pachinko.bike == null) {
			tempInput = playerInput;
			tempInput.currentActionMap.FindAction("Shoot").started += StartSolo;
		}
	}

	void StartSolo(InputAction.CallbackContext ctx) {
		RectTransform roundDisplay = roundText.transform.parent as RectTransform;
		roundDisplay.anchorMin += Vector2.up;
		roundDisplay.anchorMax += Vector2.up;
		roundDisplay.anchoredPosition = -roundDisplay.anchoredPosition;

		curLayer = -1;
		tempInput.currentActionMap.FindAction("Shoot").started -= StartSolo;
		tempInput = null;

		inputManager.DisableJoining();

		StartCoroutine(StartGame());
	}

	IEnumerator WinnerCheck() {
		houseEffect.position = Vector3.down * 100f;
		if (parlour1.score == parlour2.score) {
			winnerText.text = "no one wins";
			winnerText.color = Color.blue;

			yield return new WaitForSeconds(5f);
		}
		else {
			winnerText.text = parlour1.score > parlour2.score ? "Papa Johannes Wins" : "Large Julius Wins";
			winnerText.color = parlour1.score > parlour2.score ? Color.red : Color.green;

			PizzaBox pizza = parlour1.pachinko.manager.box;
			Transform winner = parlour1.score > parlour2.score ? parlour1.pachinko.bike.transform : parlour2.pachinko.bike.transform;
			for (float t = 0; t < 10f; t += Time.fixedDeltaTime) {
				Instantiate(pizza,
					winner.position + new Vector3(Random.Range(-10f, 10f), Random.Range(1f, 5f), Random.Range(-10f, 10f)),
					Quaternion.Euler(Random.Range(-45f, 45f), Random.Range(-45f, 45f), Random.Range(-45f, 45f))
					).Eject(true);
				yield return Bike.fixedUp;
			}
		}

		SceneManager.LoadSceneAsync(0);
	}
}
