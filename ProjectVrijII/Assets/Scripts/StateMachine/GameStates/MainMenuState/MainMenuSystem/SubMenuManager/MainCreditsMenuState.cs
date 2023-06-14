using UnityEngine;
public class MainCreditsMenuState : SubMenusBase
{
    public void ReturnToMainStartMenu()
    {
        StaticFmodCaller.staticCaller.PlayFMODEvent("event:/SfxBack");
        stateManager.SwitchState(typeof(MainStartMenuState));
    }
}
