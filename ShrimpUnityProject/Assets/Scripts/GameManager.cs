using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public PlayerInputManager inputManager;

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
        GameObject player = playerInput.gameObject;
        foreach (var cam in player.GetComponentsInChildren<CinemachineVirtualCamera>(true))
        {
            cam.gameObject.layer = curLayer;
        }
        player.GetComponentInChildren<Camera>().gameObject.layer = curLayer;
        player.GetComponentInChildren<Camera>().cullingMask = player.GetComponentInChildren<Camera>().cullingMask | (1 << curLayer);
        if (curLayer == player1Layer)
        {
            curLayer = player2Layer;
        }
    }
}
