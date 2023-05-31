using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemState : PreAttackBase {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Item selection
    /// </summary>

    // foreach item there has to be a frame containing that item and it's stats when hovering over

    [SerializeField] private GameObject buttonPrefab;

    public override void OnEnter() {
        base.OnEnter();

        buttonActions[0] += SelectReturn;

        for (int i = 0; i < character.availableItems.Count; i++) {
            // create button prefab for parent class
            // name button to item name
            // assign when clicking on item
        }

        for (int i = 0; i < character.usingItems.Count; i++) {
            // name button to item name
            // assign when clicking on item
            // unassign when clicking on item
        }
    }

    public override void OnExit() {
        base.OnExit();
        buttonActions[0] -= SelectReturn;
    }

    public void SelectReturn() {
        stateManager.SwitchState(typeof(SelectionState));
    }

    protected override void ConfirmChoice() {
        base.ConfirmChoice();
        Debug.Log("item confirmation");
    }

    private GameObject CreateButton() {
        GameObject newButton = Instantiate(buttonPrefab, menu.transform);

        // set button position based of button amount

        return newButton;
    }
}
