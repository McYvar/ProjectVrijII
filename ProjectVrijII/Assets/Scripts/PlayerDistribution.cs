using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDistribution : MonoBehaviour {
    /// <summary>
    /// Class to distribute the connected controllers over actually being a player in the game.
    /// The class allows multiple controllers to be connected, however, the first ones that press 
    /// a join button will become a player up to the playerlimit.
    /// 
    /// When a controller is connected to a player prefab, only the InputHandlers on the prefab have to be move around.
    /// </summary>


    [SerializeField] private GameObject inputController;
    [SerializeField] private int maxPlayerSlots = 2;

    private int[] playerId, deviceId;
    [HideInInspector] public int connectedPlayers;

    public Dictionary<int, InputHandler> playerInputHandlers = new Dictionary<int, InputHandler>();

    private PlayerInput[] players;


    private void Start() {
        DontDestroyOnLoad(this);

        playerId = new int[maxPlayerSlots];
        for (int i = 0; i < maxPlayerSlots; i++) {
            playerId[i] = i;
        }

        deviceId = new int[maxPlayerSlots];
        for (int i = 0; i < maxPlayerSlots; i++) {
            deviceId[i] = -1;
        }

        players = new PlayerInput[maxPlayerSlots];

        InputSystem.onDeviceChange += (device, inputDeviceChange) => {
            Debug.Log(device.name + " " + inputDeviceChange);

            if (inputDeviceChange == InputDeviceChange.Reconnected) {
                ReconnectDevice(device);
            } else if (inputDeviceChange == InputDeviceChange.Added) {
                AssignDevice(device);
            } else if (inputDeviceChange == InputDeviceChange.Removed) {
                RemoveDevice(device);
            }
        };
    }

    public void JoinPlayer(InputAction.CallbackContext cc) {
        if (cc.started && connectedPlayers < maxPlayerSlots) {
            AssignPlayer(cc.control.device);

        }
    }

    private void AssignPlayer(InputDevice device) {
        int playerIndex = FreeSlot(device);
        if (playerIndex == -1) return;
        connectedPlayers++;
        PlayerInput player = PlayerInput.Instantiate(inputController, playerIndex, "ControllerMap", -1, device);
        player.name = $"Player{connectedPlayers}";
        PlayerInput old = players[playerIndex];
        players[playerIndex] = player;
        InputHandler inputHandler = player.gameObject.GetComponent<InputHandler>();

        if (playerInputHandlers.ContainsKey(playerIndex)) { // player exists, reassign inputhandlers...
            playerInputHandlers[playerIndex].OnReassignment.Invoke(inputHandler);

            // replace the old one with the new one so if another controller connects to this player they get to control them
            Action<InputHandler> temp = playerInputHandlers[playerIndex].OnReassignment;
            playerInputHandlers[playerIndex] = inputHandler;
            inputHandler.OnReassignment = temp;
        }
        else { // player doesn't exsist yet
            playerInputHandlers.Add(playerIndex, inputHandler);
        }

        if (old != null) Destroy(old.gameObject);
    }

    private int FreeSlot(InputDevice device) {
        // check if device already exists
        for (int i = 0; i < playerId.Length; i++) {
            if (deviceId[i] == device.deviceId) return -1;
        }

        // then assign
        for (int i = 0; i < playerId.Length; i++) {
            if (deviceId[i] == -1) {
                deviceId[i] = device.deviceId;
                return playerId[i];
            }
        }
        return -1;
    }

    private void AssignDevice(InputDevice device) {
        Debug.Log($"Assigned {device.name}");
        // create prefab and assign device to Player Input
    }

    private void RemoveDevice(InputDevice device) {
        Debug.Log($"Removed {device.name}");
        for (int i = 0; i < playerId.Length; i++) {
            if (deviceId[i] == device.deviceId) {
                deviceId[i] = -1;
                connectedPlayers--;
                return;
            }
        }
    }

    private void ReconnectDevice(InputDevice device) {
        Debug.Log($"Reconnected {device.name}");
        // assign to correct device?

    }

}

//public enum 