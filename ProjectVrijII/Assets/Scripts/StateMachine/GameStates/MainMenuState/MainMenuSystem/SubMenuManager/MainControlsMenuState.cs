using UnityEngine;
public class MainControlsMenuState : SubMenusBase
{
    public void ReturnToMainStartMenu()
    {
        StaticFmodCaller.staticCaller.PlayFMODEvent("event:/SfxBack");
        stateManager.SwitchState(typeof(MainStartMenuState));
    }
}
