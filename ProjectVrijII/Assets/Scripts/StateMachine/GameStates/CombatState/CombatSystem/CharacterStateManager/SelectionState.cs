using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionState : CharacterBaseState {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Selecting trough the various options available in the selection state,
    /// for example, select attack, go to attack state
    /// </summary>

    public void SelectAttackState() {
        stateManager.SwitchState(typeof(AttackState));
    }

    public void SelectItemState() {
        stateManager.SwitchState(typeof(ItemState));
    }

    public void SelectSkillState() {
        stateManager.SwitchState(typeof(SkillState));
    }
}
