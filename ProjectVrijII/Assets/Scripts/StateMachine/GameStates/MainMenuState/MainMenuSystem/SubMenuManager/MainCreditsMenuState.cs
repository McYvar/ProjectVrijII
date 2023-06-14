public class MainCreditsMenuState : SubMenusBase
{
    public void ReturnToMainStartMenu()
    {
        stateManager.SwitchState(typeof(MainStartMenuState));
    }
}
