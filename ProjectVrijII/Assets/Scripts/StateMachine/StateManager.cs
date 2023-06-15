using UnityEngine;

public class StateManager : MonoBehaviour {
    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Updates all attached the states trough the final state machine
    /// </summary>

    protected FiniteStateMachine fsm;
    [SerializeField] protected BaseState startState;

    private void Start() {
        // on start we search for all attached BattleBaseState classes to this game object
        BaseState[] states = GetComponents<BaseState>();

        // then we couple all those states to the state machine ready for running
        fsm = new FiniteStateMachine(states, startState.GetType());
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

    protected void SwitchState(System.Type state) {
        fsm?.SwitchState(state);
    }
}
