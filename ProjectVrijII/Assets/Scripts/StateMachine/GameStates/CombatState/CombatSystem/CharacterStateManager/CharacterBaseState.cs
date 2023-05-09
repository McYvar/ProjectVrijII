using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CharacterBaseState : BaseState {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Base class containing the main functionality for the battle states
    /// </summary>

    [HideInInspector] protected PlayerInput playerInput;

    public override void OnAwake() {
        playerInput = GetComponent<PlayerInput>();
    }

    public override void OnStart() {
    }

    public override void OnEnter() {
    }

    public override void OnExit() {
    }

    public override void OnFixedUpdate() {
    }

    public override void OnLateUpdate() {
    }

    public override void OnUpdate() {
    }

    public void OnEndTurnState() {
        stateManager.SwitchState(typeof(IdleState));
    }

    public void SelectGoBackState() {
        stateManager.SwitchState(typeof(SelectionState));
    }
}
