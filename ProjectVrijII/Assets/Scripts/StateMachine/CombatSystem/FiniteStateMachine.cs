using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine {

    /// Date: 4/24/2023, by: Yvar
    /// <summary>
    /// Finite state machine that updates the current state each frame
    /// </summary>
    /// 
    private Dictionary<System.Type, BaseState> stateDictionary = new Dictionary<System.Type, BaseState>();

    private BaseState currentState;

    public FiniteStateMachine(BaseState[] states) {
        foreach (BaseState state in states) {
            state.Initialize(this);
            stateDictionary.Add(state.GetType(), state);
        }
        OnAwake();
    }

    void OnAwake() {
        foreach (BaseState state in stateDictionary.Values) {
            state.OnAwake();
        }
    }

    public void OnStart() {
        foreach (BaseState state in stateDictionary.Values) {
            state.OnStart();
        }
    }

    public void OnUpdate() {
        currentState?.OnUpdate();
    }

    public void OnFixedUpdate() {
        currentState?.OnFixedUpdate();
    }

    public void OnLateUpdate() {
        currentState?.OnLateUpdate();
    }

    public void InitState(System.Type startStateStype) {
        currentState = stateDictionary[startStateStype];
        currentState?.OnEnter();
    }

    public void SwitchState(System.Type newStateStype) {
        currentState?.OnExit();
        currentState = stateDictionary[newStateStype];
        currentState?.OnEnter();
    }

    public void DebugCurrentState() {
        Debug.Log(currentState);
    }
}
