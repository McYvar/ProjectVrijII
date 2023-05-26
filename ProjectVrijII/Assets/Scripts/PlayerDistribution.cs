using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDistribution : MonoBehaviour {
    public int maxPlayerSlots = 2;
    public GameObject inputControllerPrefab;
    public GameObject joinControllerPrefab;
    public Action OnActivePlayerDisconnected;
    public Action OnActivePlayerReconnected;

    private static PlayerDistribution instance;
    private readonly Dictionary<int, PlayerInput> allConnectedControllers = new Dictionary<int, PlayerInput>();
    private readonly Dictionary<int, PlayerInput> assignedPlayers = new Dictionary<int, PlayerInput>();
    public readonly Dictionary<int, InputHandler> playerInputHandlers = new Dictionary<int, InputHandler>();
    private readonly Dictionary<int, int> deviceToPlayerMapping = new Dictionary<int, int>();

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
        } else if (inputDeviceChange == InputDeviceChange.Reconnected) {
            //AssignDevice(device);
        }
    }

    private void AssignDevice(InputDevice device) {
        int deviceId = device.deviceId;

        if (deviceToPlayerMapping.ContainsKey(deviceId)) {
            Debug.Log($"Device({deviceId} already assigned to a join controller");
            return;
        }

        int controllerId = allConnectedControllers.Count;

        PlayerInput newPlayer = PlayerInput.Instantiate(joinControllerPrefab, controllerId, "AddControllersMap", -1, device);
        allConnectedControllers.Add(deviceId, newPlayer);
        deviceToPlayerMapping.Add(deviceId, -1);
        newPlayer.name = $"Controller{controllerId}";
    }

    private void RemoveDevice(InputDevice device) {
        int deviceId = device.deviceId;

        int playerId = deviceToPlayerMapping[deviceId];

        deviceToPlayerMapping.Remove(deviceId);
        Destroy(allConnectedControllers[deviceId].gameObject);
        allConnectedControllers.Remove(deviceId);

        if (assignedPlayers.ContainsKey(playerId)) {
            assignedPlayers[playerId] = null;
            OnActivePlayerDisconnected?.Invoke();
        }
    }


    public void AssignPlayer(InputDevice device) {
        int deviceId = device.deviceId;
        int playerId = FindFreePlayerSlot();

        if (deviceToPlayerMapping.ContainsKey(deviceId)) {
            if (deviceToPlayerMapping[deviceId] == playerId) {
                Debug.Log("Device is alread assigned to a player.");
                return;
            }
        }

        PlayerInput player = PlayerInput.Instantiate(inputControllerPrefab, playerId, "ControllerMap", -1, device);

        if (assignedPlayers.ContainsKey(playerId)) {
            if (assignedPlayers[playerId] == null) {
                assignedPlayers[playerId] = player;

                InputHandler inputHandler = player.GetComponent<InputHandler>();
                playerInputHandlers[playerId]?.OnReassignment.Invoke(inputHandler);

                // replace the old one with the new one so if another controller connects to this player they get to control them
                Action<InputHandler> temp = playerInputHandlers[playerId].OnReassignment;
                playerInputHandlers[playerId] = inputHandler;
                inputHandler.OnReassignment = temp;

                // then call that a player was reconnected
                OnActivePlayerReconnected?.Invoke();
            }
        } else {
            assignedPlayers.Add(playerId, player);
            InputHandler inputHandler = player.GetComponent<InputHandler>();
            playerInputHandlers.Add(playerId, inputHandler);
        }

        deviceToPlayerMapping[deviceId] = playerId;
        player.name = $"Player{playerId}";
    }

    public int FindFreePlayerSlot() {
        for (int i = 0; i < maxPlayerSlots; i++) {
            if (assignedPlayers.ContainsKey(i)) {
                if (assignedPlayers[i] == null) return i;
            } else return i;
        }
        return -1;
    }
}
