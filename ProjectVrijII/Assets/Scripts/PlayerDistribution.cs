using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDistribution : MonoBehaviour {
    public int maxPlayerSlots = 2;
    public GameObject inputControllerPrefab;
    public GameObject joinControllerPrefab;
    public Action<int, ControllerType> OnActivePlayerDisconnected;
    public Action<int, ControllerType> OnActivePlayerReconnected;

    private static PlayerDistribution instance;
    private readonly Dictionary<int, PlayerInput> allConnectedControllers = new Dictionary<int, PlayerInput>();
    private readonly Dictionary<int, PlayerInput> assignedPlayers = new Dictionary<int, PlayerInput>();
    private readonly Dictionary<int, InputHandler> playerInputHandlers = new Dictionary<int, InputHandler>();
    private readonly Dictionary<int, int> deviceToPlayerMapping = new Dictionary<int, int>();
    private readonly Dictionary<int, ControllerType> connectedControllerTypes = new Dictionary<int, ControllerType>();

    public static PlayerDistribution Instance {
        get { return instance; }
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        InputSystem.onDeviceChange += OnDeviceChange;

        foreach (var device in InputSystem.devices) {
            AssignDevice(device);
        }
    }

    private void OnDestroy() {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange inputDeviceChange) {
        if (inputDeviceChange == InputDeviceChange.Added) {
            AssignDevice(device);
        } else if (inputDeviceChange == InputDeviceChange.Removed) {
            RemoveDevice(device);
        }
    }

    private void AssignDevice(InputDevice device) {
        int deviceId = device.deviceId;

        if (deviceToPlayerMapping.ContainsKey(deviceId)) {
            Debug.Log($"Device({deviceId} already assigned to a join controller");
            return;
        }

        PlayerInput newPlayer = PlayerInput.Instantiate(joinControllerPrefab, deviceId, "AddControllersMap", -1, device);
        allConnectedControllers.Add(deviceId, newPlayer);
        deviceToPlayerMapping.Add(deviceId, -1);

        ControllerType controllerType = GetControllerType(device.description.ToString());
        connectedControllerTypes.Add(deviceId, controllerType);
        Debug.Log(controllerType.ToString());

        newPlayer.name = $"Controller{deviceId}({controllerType})";
    }

    private void RemoveDevice(InputDevice device) {
        int deviceId = device.deviceId;

        int playerId = deviceToPlayerMapping[deviceId];

        deviceToPlayerMapping.Remove(deviceId);
        Destroy(allConnectedControllers[deviceId].gameObject);
        allConnectedControllers.Remove(deviceId);

        if (assignedPlayers.ContainsKey(playerId)) {
            assignedPlayers[playerId] = null;
            OnActivePlayerDisconnected?.Invoke(playerId, connectedControllerTypes[deviceId]);
        }
    }


    public void AssignPlayer(InputDevice device) {
        int deviceId = device.deviceId;

        if (deviceToPlayerMapping[deviceId] != -1) {
            Debug.Log($"Device ({deviceId}) already mapped to a player!");
            return;
        }

        int playerId = FindFreePlayerSlot();
        if (playerId == -1) {
            Debug.Log($"No player left to assign to; device: {deviceId}");
            return;
        }

        PlayerInput player = PlayerInput.Instantiate(inputControllerPrefab, playerId, "ControllerMap", -1, device);

        if (assignedPlayers.ContainsKey(playerId)) {
            if (assignedPlayers[playerId] == null) {
                assignedPlayers[playerId] = player;

                InputHandler inputHandler = player.GetComponent<InputHandler>();
                playerInputHandlers[playerId]?.OnReassignment.Invoke(inputHandler);

                // replace the old one with the new one so if another controller connects to this player they get to control them
                Action<InputHandler> temp = playerInputHandlers[playerId].OnReassignment;
                Destroy(playerInputHandlers[playerId].gameObject);
                playerInputHandlers[playerId] = inputHandler;
                inputHandler.OnReassignment = temp;

                // then call that a player was reconnected
                OnActivePlayerReconnected?.Invoke(playerId, connectedControllerTypes[deviceId]);
            }
        } else {
            assignedPlayers.Add(playerId, player);
            InputHandler inputHandler = player.GetComponent<InputHandler>();
            playerInputHandlers.Add(playerId, inputHandler);
        }

        deviceToPlayerMapping[deviceId] = playerId;
        player.name = $"Player{playerId}({connectedControllerTypes[deviceId]})";
    }

    public int FindFreePlayerSlot() {
        for (int i = 0; i < maxPlayerSlots; i++) {
            if (assignedPlayers.ContainsKey(i)) {
                if (assignedPlayers[i] == null) return i;
            } else return i;
        }
        return -1;
    }

    private ControllerType GetControllerType(string deviceDescription) {
        Debug.Log(deviceDescription);
        if (deviceDescription.Contains("Keyboard")) {
            return ControllerType.Keyboard;
        } else if (deviceDescription.Contains("Mouse")) {
            return ControllerType.Mouse;
        } else if (deviceDescription.Contains("Sony")) {
            return ControllerType.PS4;
        } else if (deviceDescription.Contains("XBox")) {
            return ControllerType.Xbox;
        }

        return ControllerType.Unknown;
    }

    public void SubscribeToPlayerInputHandler(int playerId, Action<InputHandler> callback) {
        playerInputHandlers[playerId].OnReassignment += callback;
    }

    public void UnSubscribeFromPlayerInputHandler(int playerId, Action<InputHandler> callback) {
        playerInputHandlers[playerId].OnReassignment -= callback;
    }

    public InputHandler GetPlayerInputHandler(int playerId) {
        return playerInputHandlers[playerId];
    }

    public int GetAssignedPlayersCount() {
        return assignedPlayers.Count;
    }
}
public enum ControllerType {
    Unknown = -1,
    Keyboard = 0,
    Mouse = 1,
    PS4 = 2,
    Xbox = 3,
    // Add more controller types as needed
}