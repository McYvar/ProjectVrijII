using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BattleBaseState : BaseState {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Base class containing the main functionality for the battle states
    /// </summary>

    public override void OnAwake() {
        Debug.Log("Awake " + this);
    }

    public override void OnStart() {
        Debug.Log("Start " + this);
    }

    public override void OnEnter() {
        Debug.Log("Entered " + this);
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
