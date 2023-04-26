using UnityEngine;

public class StateManager : MonoBehaviour {
    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Updates all attached the states trough the final state machine
    /// </summary>

    [SerializeField] private bool dontDestroyOnLoad = false;
    public bool isEnabled = true;

    private FiniteStateMachine fsm;
    [SerializeField] private BaseState startState;

    private void Awake() {
        if (dontDestroyOnLoad) DontDestroyOnLoad(this);

        // on start we search for all attached BattleBaseState classes to this game object
        BaseState[] states = GetComponents<BaseState>();

        // then we couple all those states to the state machine ready for running
        fsm = new FiniteStateMachine(states);
    }

    private void Start() {
        fsm?.OnStart();
        fsm.InitState(startState.GetType());
    }

    private void Update() {
        if (isEnabled) fsm?.OnUpdate();
    }

    private void FixedUpdate() {
        if (isEnabled) fsm?.OnFixedUpdate();
    }

    private void LateUpdate() {
        if (isEnabled) fsm?.OnLateUpdate();
    }

    public void SwitchState(System.Type state) {
        if (isEnabled) fsm?.SwitchState(state);
    }
}
