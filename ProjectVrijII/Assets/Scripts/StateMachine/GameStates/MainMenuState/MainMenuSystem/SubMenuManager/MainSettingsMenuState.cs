using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSettingsMenuState : SubMenusBase
{
    public void ReturnToMainStartMenu() {
        stateManager.SwitchState(typeof(MainStartMenuState));
    }
}
