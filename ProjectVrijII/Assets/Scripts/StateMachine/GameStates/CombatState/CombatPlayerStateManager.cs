using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatPlayerStateManager : StateManager {
    private void OnEnable() => CombatState.managers.Add(this);
    private void OnDisable() => CombatState.managers.Remove(this);

    [HideInInspector] public bool isEnabled = false;
    
    protected override void Update() {
        if (isEnabled) fsm?.OnUpdate();
    }

    protected override void FixedUpdate() {
        if (isEnabled) fsm?.OnFixedUpdate();
    }

    protected override void LateUpdate() {
        if (isEnabled) fsm?.OnLateUpdate();
    }
}