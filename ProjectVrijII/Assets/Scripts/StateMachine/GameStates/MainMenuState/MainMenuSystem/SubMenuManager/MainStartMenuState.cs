using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainStartMenuState : SubMenusBase
{
    public void PressStartButton(int scenenumber) {
        PlayerDistribution.Instance.ResetInputHandlers();
        SceneManager.LoadScene(scenenumber);
    }

    public void PressSettingsButton() {
        stateManager.SwitchState(typeof(MainSettingsMenuState));
    }
}
