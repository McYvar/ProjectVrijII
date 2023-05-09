using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : BaseState {

    public static List<CombatBase> combatClasses = new List<CombatBase>();

    public override void OnAwake() {
    }

    public override void OnStart() {
    }

    public override void OnEnter() {
    }

    public override void OnExit() {
    }

    public override void OnUpdate() {
        foreach (var combatClass in combatClasses) {
            combatClass.OnUpdate();
        }
    }

    public override void OnFixedUpdate() {
        foreach(var combatClass in combatClasses) {
            combatClass.OnFixedUpdate();
        }
    }

    public override void OnLateUpdate() {
        foreach (var combatClass in combatClasses) {
            combatClass.OnLateUpdate();
        }
    }
}
