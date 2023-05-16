using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatState : BaseState {

    public static Queue<CombatBase> combatAddQueue = new Queue<CombatBase>();
    public static Queue<CombatBase> combatRemoveQueue = new Queue<CombatBase>();
    public static List<CombatBase> combatClasses = new List<CombatBase>();

    public override void OnEnter() {
    }

    public override void OnExit() {
    }

    public override void OnUpdate() {
        foreach (var combatClass in combatClasses) {
            combatClass.OnUpdate();
        }

        while (combatAddQueue.Count > 0)
        {
            combatClasses.Add(combatAddQueue.Dequeue());
        }

        while (combatRemoveQueue.Count > 0)
        {
            combatClasses.Remove(combatRemoveQueue.Dequeue());
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
