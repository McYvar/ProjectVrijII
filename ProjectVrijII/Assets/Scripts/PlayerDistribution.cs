using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDistribution : MonoBehaviour
{
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private GameObject inputController;

    private void Start() {
        InputSystem.onDeviceChange += (device, inputDeviceChange) => {
            Debug.Log(device.name + " " + inputDeviceChange);

            if (inputDeviceChange == InputDeviceChange.Reconnected) {
                ReconnectDevice(device);
            }
            else if (inputDeviceChange == InputDeviceChange.Added) {
                AssignDevice(device);
            }
            else if (inputDeviceChange == InputDeviceChange.Removed) {
                RemoveDevice(device);
            }

        };
    }

    private void Update()
    {

    }

    private void AssignDevice(InputDevice device) {
        Debug.Log($"Assigned {device.name}");
        // create prefab and assign device to Player Input

        GameObject newController = Instantiate(inputController, Vector3.zero, Quaternion.identity);
    }

    private void RemoveDevice(InputDevice device) {
        Debug.Log($"Removed {device.name}");
    }

    private void ReconnectDevice(InputDevice device) {
        Debug.Log($"Reconnected {device.name}");
        // assign to correct device?
    }

    public void OnPlayerJoined(PlayerInput playerInput) {
        Debug.Log("player joined!");
    }

    
}

//public enum 