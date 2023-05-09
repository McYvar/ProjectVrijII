using UnityEngine;

public class StateManager : MonoBehaviour {
    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Updates all attached the states trough the final state machine
    /// </summary>

    protected FiniteStateMachine fsm;
    [SerializeField] protected BaseState startState;

    private void Awake() {
        DontDestroyOnLoad(this);

        // on start we search for all attached BattleBaseState classes to this game object
        BaseState[] states = GetComponents<BaseState>();

        // then we couple all those states to the state machine ready for running
        fsm = new FiniteStateMachine(states);
    }

    private void Start() {
        fsm?.OnStart();
        fsm.InitState(startState.GetType());
        Debug.Log(gameObject);
    }

    protected virtual void Update() {
        fsm?.OnUpdate();
    }

    protected virtual void FixedUpdate() {
        fsm?.OnFixedUpdate();
    }

    protected virtual void LateUpdate() {
        fsm?.OnLateUpdate();
    }

    protected void SwitchState(System.Type state) {
        fsm?.SwitchState(state);
    }
}
