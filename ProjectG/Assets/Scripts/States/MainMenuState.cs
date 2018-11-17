using UnityEngine.SceneManagement;

public class MainMenuState : GameState
{
    protected override void OnStart()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnSceneChanged;
        SceneManager.LoadScene("MainMenu");
    }

    protected override void OnEnd()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    protected override void OnUpdate()
    {
    }

    protected override void OnFixedUpdate()
    {
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        UIManager.Instance.MainMenu.ShowPanel();
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        SceneManager.SetActiveScene(loadedScene);
    }
}
