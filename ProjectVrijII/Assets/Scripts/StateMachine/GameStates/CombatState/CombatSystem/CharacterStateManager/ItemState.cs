using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemState : PreAttackBase {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Item selection
    /// </summary>

    // foreach item there has to be a frame containing that item and it's stats when hovering over

    public override void OnEnter() {
        base.OnEnter();

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
    }

    public void SelectReturn() {
        stateManager.SwitchState(typeof(SelectionState));
    }

    protected override void ConfirmChoice() {
        base.ConfirmChoice();
        Debug.Log("item confirmation");
    }
}
