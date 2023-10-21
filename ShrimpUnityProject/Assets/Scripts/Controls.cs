using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controls : MonoBehaviour
{
    public static PlayerActions controls;
    public static void Init()
    {
        if (controls != null) return;

        controls = new PlayerActions();
        controls.Enable();
    }

}
