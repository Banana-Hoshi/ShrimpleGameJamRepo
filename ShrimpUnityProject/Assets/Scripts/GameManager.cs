using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public PlayerInputManager inputManager;
	public PizzaParlour parlour1;
	public PizzaParlour parlour2;
	public Camera cam;

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
			parlour1.pachinko.bike.enabled = true;
			parlour2.pachinko.bike.enabled = true;

			curLayer = -1;
		}
    }
}
