public class MainControlsMenuState : SubMenusBase
{
    public void ReturnToMainStartMenu()
    {
        stateManager.SwitchState(typeof(MainStartMenuState));
    }
}
