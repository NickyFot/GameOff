using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuState : GameState
{
    protected override void OnStart()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnSceneChanged;
        SceneManager.LoadScene("MainMenu");

        AudioManager.Instance.Play2DAudio(Resources.Load<AudioClip>("Audio/Ambience/Sea Waves"), AudioManager.ChannelType.AMBIENCE, true);
        AudioManager.Instance.SetChannelVolume(AudioManager.ChannelType.AMBIENCE, 0);
        AudioManager.Instance.FadeChannel(AudioManager.ChannelType.AMBIENCE, 0.3f, 1);

        AudioManager.Instance.Play2DAudio(Resources.Load<AudioClip>("Audio/Blow_The_Man_Down"), AudioManager.ChannelType.MUSIC, true);
        AudioManager.Instance.SetChannelVolume(AudioManager.ChannelType.MUSIC, 0);
        AudioManager.Instance.FadeChannel(AudioManager.ChannelType.MUSIC, 1, 2);

        //debug
        AudioManager.Instance.Play2DAudio(Resources.Load<AudioClip>("Audio/Ambience/flock-of-seagulls"), AudioManager.ChannelType.AMBIENTTWO, true);
        AudioManager.Instance.SetChannelVolume(AudioManager.ChannelType.AMBIENTTWO, 0);
        AudioManager.Instance.FadeChannel(AudioManager.ChannelType.AMBIENTTWO, 0.3f, 1);
    }

    protected override void OnEnd()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnSceneChanged;
        AudioManager.Instance.FadeChannel(AudioManager.ChannelType.MUSIC, 0, 1.5f, delegate { AudioManager.Instance.StopChannel(AudioManager.ChannelType.MUSIC); });

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
