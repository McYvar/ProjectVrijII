using UnityEngine;

public class SelectionState : PreAttackBase {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Selecting trough the various options available in the selection state,
    /// for example, select attack, go to attack state
    /// </summary>

    public override void OnEnter() {
        base.OnEnter();
        buttonActions[0] += SelectAttackState;
        buttonActions[1] += SelectItemState;
    }

    public void SelectAttackState() {
        stateManager.SwitchState(typeof(OnGroundMovement));
    }

    public void SelectItemState() {
        stateManager.SwitchState(typeof(ItemState));
    }

    public void SelectSkillState() {
        stateManager.SwitchState(typeof(SkillState));
    }

    protected override void ConfirmChoice() {
        base.ConfirmChoice();
        Debug.Log("selection confirmation");
    }
}
