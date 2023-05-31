using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBase : BaseState {

    private void OnEnable() => MainMenuState.mainMenuAddQueue.Enqueue(this);
    private void OnDisable() => MainMenuState.mainMenuRemoveQueue.Enqueue(this);

    public override void OnEnter() {
    }

    public override void OnExit() {
    }

    public override void OnFixedUpdate() {
    }

    public override void OnLateUpdate() {
    }

    public override void OnUpdate() {
        OnExit();
        if (MainMenuState.mainMenuClasses.Contains(this)) MainMenuState.mainMenuClasses.Remove(this);
    }
}
