using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateManager : CombatBase {

    /// <summary>
    /// Seperate state manager that is run by the combat system instead of monobehaviour updates
    /// </summary>

    protected FiniteStateMachine fsm;
    [SerializeField] protected BaseState startState;

    private void Awake() {
        //DontDestroyOnLoad(this);

        // on start we search for all attached BattleBaseState classes to this game object
        BaseState[] states = GetComponents<BaseState>();

        // then we couple all those states to the state machine ready for running
        fsm = new FiniteStateMachine(states, startState.GetType());
    }

    public override void OnUpdate() {
        fsm?.OnUpdate();
    }

    public override void OnFixedUpdate() {
        fsm?.OnFixedUpdate();
    }

    public override void OnLateUpdate() {
        fsm?.OnLateUpdate();
    }

    protected void SwitchState(System.Type state) {
        fsm?.SwitchState(state);
    }
}
