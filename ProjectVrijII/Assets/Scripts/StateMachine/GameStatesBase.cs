using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatesBase : BaseState {
    /// Date: 4/26/2023, by: Yvar
    /// <summary>
    /// Seperates the game flow into states
    /// </summary>
    
    [SerializeField] protected List<StateManager> managers = new List<StateManager>();

    public override void OnAwake() {
    }

    public override void OnStart() {
    }

    public override void OnEnter() {
        foreach (var manager in managers) {
            manager.isEnabled = true;
        }
    }

    public override void OnExit() {
        foreach (var manager in managers) {
            manager.isEnabled = false;
        }
    }

    public override void OnUpdate() {
    }

    public override void OnFixedUpdate() {
    }

    public override void OnLateUpdate() {
    }
}
