using UnityEngine;

public class CharacterAssignment : CombatBase {
    [SerializeField] bool instantStart = false; // for test build
    [SerializeField] private GameObject characterController;
    [SerializeField] private SO_Character[] characters;
    [SerializeField] private Vector3[] spawns;
    [SerializeField] private CameraBehaviour cameraBehaviour;

    private GameObject[] activeCharacters = new GameObject[2];

    [SerializeField] GameObject UI_PlayerDisconectedWindow;
    [SerializeField] TMPro.TMP_Text disconnectedPlayerCounter;

    private void Start() {
        PlayerDistribution.Instance.OnActivePlayerReconnected += CharacterReconnected;
        PlayerDistribution.Instance.OnActivePlayerDisconnected += CharacterLostConnection;
    }

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.Backspace) || instantStart) { // spawns the characters for now
            instantStart = false;
            AssignToCharacters();

            if (activeCharacters.Length >= 2) {
                CharacterBaseState[] characterBaseStates = activeCharacters[0].GetComponents<CharacterBaseState>();
                foreach (var characterState in characterBaseStates) {
                    characterState.SetEnemy(activeCharacters[1].transform);
                }
                cameraBehaviour.AssignObjects(activeCharacters[0].transform);

                characterBaseStates = activeCharacters[1].GetComponents<CharacterBaseState>();
                foreach (var characterState in characterBaseStates) {
                    characterState.SetEnemy(activeCharacters[0].transform);
                }
                cameraBehaviour.AssignObjects(activeCharacters[1].transform);
            }

            TurnSystem.Instance.InitializeCharacters(activeCharacters[0].GetComponent<CharacterStateManager>(), activeCharacters[1].GetComponent<CharacterStateManager>());
            TurnSystem.Instance.StartCombat();
        }
    }

    // This method is supposed to be called when the players spawn in
    private void AssignToCharacters() {
        for (int i = 0; i < activeCharacters.Length; i++) {
            GameObject characterObj = Instantiate(characterController, spawns[i], Quaternion.identity);
            characterObj.GetComponent<CharacterStateManager>().ManualInitialize();
            characterObj.name = characters[i].name;

            CharacterBaseState[] characterBaseStates = characterObj.GetComponents<CharacterBaseState>();
            foreach (var characterState in characterBaseStates) {
                InputHandler inputHandler = PlayerDistribution.Instance.GetPlayerInputHandler(i);
                if (inputHandler != null) { // null check should also be temp to make a playable build for the team
                    characterState.SetInputHandler(inputHandler);
                    // add a method to the action on the inputHandler to assure good reassignment
                    PlayerDistribution.Instance.SubscribeToPlayerInputHandler(i, characterState.SetInputHandler);
                }
                // temp...
                characterState.SetCharacter(characters[i]);
            }

            // also temp...
            activeCharacters[i] = characterObj;
        }
    }

    private void CharacterLostConnection(int deviceId, ControllerType controllerType) {
        int assignedPlayerCount = PlayerDistribution.Instance.GetAssignedPlayersCount();
        disconnectedPlayerCounter.text = $"Disconnected players: {activeCharacters.Length - assignedPlayerCount}";
        UI_PlayerDisconectedWindow.SetActive(true);
        Debug.Log($"{deviceId} lost connection; {controllerType}");
        Time.timeScale = 0;
    }

    private void CharacterReconnected(int deviceId, ControllerType controllerType) {
        int assignedPlayerCount = PlayerDistribution.Instance.GetAssignedPlayersCount();
        disconnectedPlayerCounter.text = $"Disconnected players: {activeCharacters.Length - assignedPlayerCount}";
        if (assignedPlayerCount >= activeCharacters.Length) {
            Debug.Log($"{deviceId} reconnected; {controllerType}");
            UI_PlayerDisconectedWindow.SetActive(false);
            Time.timeScale = 1;
        }
    }
}