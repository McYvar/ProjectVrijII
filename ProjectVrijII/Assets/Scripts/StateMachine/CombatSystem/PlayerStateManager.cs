using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Updates all attached the states trough the final state machine
    /// </summary>

    private FiniteStateMachine fsm;
    [SerializeField] private PlayerBaseState startState;

    private void Awake() {
        // on start we search for all attached BattleBaseState classes to this game object
        PlayerBaseState[] states = GetComponents<PlayerBaseState>();

        // then we couple all those states to the state machine ready for running
        fsm = new FiniteStateMachine(states);
    }

    private void Start() {
        fsm?.OnStart();
        fsm.InitState(startState.GetType());
    }

    private void Update() {
        fsm?.OnUpdate();
    }

    private void FixedUpdate() {
        fsm?.OnFixedUpdate();
    }

    private void LateUpdate() {
        fsm?.OnLateUpdate();
    }

    public void SwitchState(System.Type state) {
        fsm?.SwitchState(state);
    }
}
