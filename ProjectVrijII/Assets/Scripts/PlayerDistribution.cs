using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class PlayerDistribution : MonoBehaviour {
    public int maxPlayerSlots = 2;
    public GameObject inputControllerPrefab;
    public GameObject joinControllerPrefab;
    public Action<int, ControllerType> OnActivePlayerDisconnected;
    public Action<int, ControllerType> OnActivePlayerReconnected;

    private readonly Dictionary<int, PlayerInput> allConnectedControllers = new Dictionary<int, PlayerInput>();
    private readonly Dictionary<int, PlayerInput> assignedPlayers = new Dictionary<int, PlayerInput>();
    private readonly Dictionary<int, Color> assignedPlayersColors = new Dictionary<int, Color>();
    private readonly Dictionary<int, InputHandler> playerInputHandlers = new Dictionary<int, InputHandler>();
    private readonly Dictionary<int, int> deviceToPlayerMapping = new Dictionary<int, int>(); // deviceId, playerId
    private readonly Dictionary<int, int> playerToDeviceMapping = new Dictionary<int, int>(); // playerId, deviceId
    private readonly Dictionary<int, ControllerType> allConnectedControllersType = new Dictionary<int, ControllerType>();

    [SerializeField] private Color[] playerColors;

    private static PlayerDistribution instance;
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

        ControllerType controllerType = GetControllerType(device);
        allConnectedControllersType.Add(deviceId, controllerType);

        newPlayer.name = $"Controller{deviceId}({controllerType})";
    }

    private void RemoveDevice(InputDevice device) {
        int deviceId = device.deviceId;

        int playerId = deviceToPlayerMapping[deviceId];

        if (assignedPlayers.ContainsKey(playerId)) {
            assignedPlayers[playerId] = null;
            OnActivePlayerDisconnected?.Invoke(playerId, allConnectedControllersType[deviceId]); // invoke disconnection action with player ID and controller type
            playerToDeviceMapping.Remove(playerId);
            assignedPlayersColors.Remove(playerId);
        }

        Destroy(allConnectedControllers[deviceId].gameObject);
        allConnectedControllers.Remove(deviceId);
        deviceToPlayerMapping.Remove(deviceId);
        allConnectedControllersType.Remove(deviceId);

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

                // create a new handler
                InputHandler inputHandler = player.GetComponent<InputHandler>();

                // replace the old one with the new one so if another controller connects to this player they get to control them
                playerInputHandlers[playerId].CopyTo(inputHandler);
                Destroy(playerInputHandlers[playerId].gameObject);
                playerInputHandlers[playerId] = inputHandler;

                // then rebind the controlls to all objects that require it and call that a player was reconnected
                inputHandler.OnReassignment?.Invoke(inputHandler);
                OnActivePlayerReconnected?.Invoke(playerId, allConnectedControllersType[deviceId]);
            }
        } else {
            assignedPlayers.Add(playerId, player);
            InputHandler inputHandler = player.GetComponent<InputHandler>();
            playerInputHandlers.Add(playerId, inputHandler);
        }

        deviceToPlayerMapping[deviceId] = playerId;
        playerToDeviceMapping.Add(playerId, deviceId);
        player.name = $"Player{playerId}({allConnectedControllersType[deviceId]})";
        SetPlayerColor(playerId, playerColors[playerId]);
    }

    public int FindFreePlayerSlot() {
        for (int i = 0; i < maxPlayerSlots; i++) {
            if (assignedPlayers.ContainsKey(i)) {
                if (assignedPlayers[i] == null) return i;
            } else return i;
        }
        return -1;
    }

    private ControllerType GetControllerType(InputDevice device) {
        string deviceDescription = device.description.ToString();
        if (deviceDescription.Contains("Keyboard")) {
            return ControllerType.Keyboard;
        } else if (deviceDescription.Contains("Mouse")) {
            return ControllerType.Mouse;
        } else if (deviceDescription.Contains("Sony")) {
            var controller = (DualShockGamepad)device;
            if (controller != null) controller.SetLightBarColor(new Color(255, 255, 255));
            return ControllerType.PS;
        } else if (deviceDescription.Contains("XBox")) {
            return ControllerType.Xbox;
        }
        else {
            Debug.Log($"Description: {deviceDescription}");
            return ControllerType.Unknown;
        }
    }

    public void SubscribeToPlayerInputHandler(int playerId, Action<InputHandler> callback) {
        playerInputHandlers[playerId].OnReassignment += callback;
    }

    public void UnSubscribeFromPlayerInputHandler(int playerId, Action<InputHandler> callback) {
        playerInputHandlers[playerId].OnReassignment -= callback;
    }

    public InputHandler GetPlayerInputHandler(int playerId) {
        if (playerInputHandlers.ContainsKey(playerId)) {
            return playerInputHandlers[playerId];
        }
        else {
            PlayerInput dummy = PlayerInput.Instantiate(inputControllerPrefab, playerId, "ControllerMap", -1);
            assignedPlayers.Add(playerId, dummy);
            InputHandler inputHandler = dummy.GetComponent<InputHandler>();
            playerInputHandlers.Add(playerId, inputHandler);
            assignedPlayersColors.Add(playerId, new Color(150, 150, 150));
            Debug.Log("spawned dummy");
            return inputHandler;
        }
    }

    public int GetAssignedPlayersCount() {
        int result = 0;
        foreach (var p in assignedPlayers.Values) {
            if (p != null) result++;
        }
        return result;
    }

    public void SetPlayerColor(int playerId, Color color) {
        Debug.Log(playerId);
        int deviceId = playerToDeviceMapping[playerId];
        assignedPlayersColors.Add(playerId, color);

        if (allConnectedControllers.ContainsKey(deviceId)) {
            if (allConnectedControllersType[deviceId] == ControllerType.PS) {
                if (allConnectedControllers[deviceId].devices.Count > 0) {
                    var controller = (DualShockGamepad)allConnectedControllers[deviceId].devices[0];
                    if (controller != null) {
                        controller.SetLightBarColor(color);
                    }
                }
            }
        }
    }

    public Color GetPlayerColor(int playerId) {
        return assignedPlayersColors[playerId];
    }

    public void ResetInputHandlers() {
        for (int i = 0; i < playerInputHandlers.Count; i++) {
            playerInputHandlers[i].ResetBindings();
        }
    }
}
public enum ControllerType {
    Unknown = -1,
    Keyboard = 0,
    Mouse = 1,
    PS = 2,
    Xbox = 3,
    // Add more controller types as needed
}