using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterAssignment : CombatBase
{
    [SerializeField] private GameObject characterController;
    [SerializeField] private SO_Character[] characters;
    [SerializeField] private Vector3[] spawns;
    [SerializeField] private CameraBehaviour cameraBehaviour;

    private GameObject[] activeCharacters = new GameObject[2];

    [SerializeField] GameObject UI_PlayerDisconectedWindow;

    private void Start() {
        PlayerDistribution.Instance.OnActivePlayerReconnected += CharacterReconnected;
        PlayerDistribution.Instance.OnActivePlayerDisconnected += CharacterLostConnection;
    }

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
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
        }
    }

    // This method is supposed to be called when the players spawn in
    private void AssignToCharacters() {
        for (int i = 0; i < activeCharacters.Length; i++) {
            GameObject characterObj = Instantiate(characterController, spawns[i], Quaternion.identity);
            characterObj.name = characters[i].name;

            CharacterBaseState[] characterBaseStates = characterObj.GetComponents<CharacterBaseState>();
            foreach (var characterState in characterBaseStates) {
                characterState.SetInputHandler(PlayerDistribution.Instance.playerInputHandlers[i]);

                // add a method to the action on the inputHandler to assure good reassignment
                PlayerDistribution.Instance.playerInputHandlers[i].OnReassignment += characterState.SetInputHandler;

                // temp...
                characterState.SetCharacter(characters[i]);
            }

            // also temp...
            activeCharacters[i] = characterObj;
        }
    }

    private void CharacterLostConnection() {
        UI_PlayerDisconectedWindow.SetActive(true);
        Time.timeScale = 0;
    }

    private void CharacterReconnected() {
        if (PlayerDistribution.Instance.playerInputHandlers.Count >= activeCharacters.Length) {
            UI_PlayerDisconectedWindow.SetActive(false);
            Time.timeScale = 1;
        }
    }
}
