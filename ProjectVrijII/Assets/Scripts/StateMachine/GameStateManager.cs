public class GameStateManager : StateManager
{
    private static GameStateManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }
}