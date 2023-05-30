using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBase : BaseState {

    private void OnEnable() => CombatState.combatAddQueue.Enqueue(this);
    private void OnDisable() => CombatState.combatRemoveQueue.Enqueue(this);

    public override void OnEnter() {
    }

    public override void OnExit() {
    }

    public override void OnUpdate() {
    }

    public override void OnFixedUpdate() {
    }

    public override void OnLateUpdate() {
    }

    private void OnDestroy() {
        // on destroy force remove class from combatState
        OnExit();
        if (CombatState.combatClasses.Contains(this)) CombatState.combatClasses.Remove(this);
    }
}
