using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuState : BaseState {

    public static Queue<MainMenuBase> mainMenuAddQueue = new Queue<MainMenuBase>();
    public static Queue<MainMenuBase> mainMenuRemoveQueue = new Queue<MainMenuBase>();
    public static List<MainMenuBase> mainMenuClasses = new List<MainMenuBase>();

    public override void OnEnter() {
    }

    public override void OnExit() {
        foreach (var menu in mainMenuClasses) {
            menu.OnUpdate();
        }

        while (mainMenuAddQueue.Count > 0) {
            MainMenuBase toAdd = mainMenuAddQueue.Dequeue();
            if (toAdd != null) mainMenuClasses.Add(toAdd);
        }

        while (mainMenuRemoveQueue.Count > 0) {
            MainMenuBase toAdd = mainMenuRemoveQueue.Dequeue();
            if (toAdd != null) mainMenuClasses.Remove(toAdd);
        }
    }
    public override void OnUpdate() {
        foreach (var menu in mainMenuClasses) {
            menu.OnUpdate();
        }
    }

    public override void OnFixedUpdate() {
        foreach (var menu in mainMenuClasses) {
            menu.OnFixedUpdate();
        }
    }

    public override void OnLateUpdate() {
        foreach (var menu in mainMenuClasses) {
            menu.OnLateUpdate();
        }
    }

    public void SwitchToCombatState() {
        stateManager.SwitchState(typeof(CombatState));
    }
}
