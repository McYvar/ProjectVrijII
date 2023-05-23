using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDistribution : MonoBehaviour
{
    [SerializeField] private PlayerInputManager inputManager;

    private InputDevice[] inputDevices;

    private void Start()
    {
        inputManager.onPlayerJoined += (d) => { Debug.Log("kjhk"); };
    }

    private void Update()
    {
        
    }
}
