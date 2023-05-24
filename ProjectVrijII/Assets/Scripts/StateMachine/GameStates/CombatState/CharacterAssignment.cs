using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAssignment : CombatBase
{
    [SerializeField] GameObject characterController;
    [SerializeField] SO_Character[] characters;
    [SerializeField] Vector3[] spawns;
    [SerializeField] PlayerDistribution playerDistribution;
    [SerializeField] CameraBehaviour cameraBehaviour;

    public override void OnUpdate() {
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            AssignToCharacters();
        }
    }

    private void AssignToCharacters() {
        for (int i = 0; i < playerDistribution.connectedPlayers; i++) {
            GameObject characterObj = Instantiate(characterController, spawns[i], Quaternion.identity);
            characterObj.name = characters[i].name;

            CharacterBaseState[] characterBaseStates = characterObj.GetComponents<CharacterBaseState>();
            foreach (var characterState in characterBaseStates) {
                characterState.PlayerAssignment(playerDistribution.inputHandlers[i], characters[i]);
            }

            cameraBehaviour.AssignObjects(characterObj.transform);
        }
    }
}
