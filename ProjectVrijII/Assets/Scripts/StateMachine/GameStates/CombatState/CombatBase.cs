using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatBase : BaseState {

    private void OnEnable() => CombatState.combatClasses.Add(this);
    private void OnDisable() => CombatState.combatClasses.Remove(this);

    public override void OnAwake() {
    }

    public override void OnStart() {
    }

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
}
