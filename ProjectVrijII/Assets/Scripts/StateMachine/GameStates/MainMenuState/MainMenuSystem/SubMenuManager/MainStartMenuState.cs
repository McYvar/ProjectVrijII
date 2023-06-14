using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MainStartMenuState : SubMenusBase
{
    public void PressStartButton(int scenenumber) {
        StaticFmodCaller.staticCaller.PlayFMODEvent("event:/SfxSelection");
        if (PlayerDistribution.Instance.GetAssignedPlayersCount() >= 2)
        {
            PlayerDistribution.Instance.ResetInputHandlers();
            SceneManager.LoadScene(scenenumber);
        }
    }

    public void PressControlsButton()
    {
        StaticFmodCaller.staticCaller.PlayFMODEvent("event:/SfxSelection");
        stateManager.SwitchState(typeof(MainControlsMenuState));
    }

    public void PressCreditsButton()
    {
        StaticFmodCaller.staticCaller.PlayFMODEvent("event:/SfxSelection");
        stateManager.SwitchState(typeof(MainCreditsMenuState));
    }

    public void PressQuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
