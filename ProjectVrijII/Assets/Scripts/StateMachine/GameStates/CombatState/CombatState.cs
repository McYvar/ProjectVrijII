using System.Collections.Generic;
using UnityEngine;

public class CombatState : GameStatesBase {
    /// Date: 4/26/2023, by: Yvar
    /// <summary>
    /// Combat state manages all combat related states and actions
    /// </summary>

    public static List<CombatPlayerStateManager> managers = new List<CombatPlayerStateManager>();

    public override void OnEnter() {
        base.OnEnter();
        foreach (var manager in managers) {
            manager.isEnabled = true;
        }
    }

    public override void OnExit() {
        base.OnExit();
        foreach (var manager in managers) {
            manager.isEnabled = false;
        }
    }
}
