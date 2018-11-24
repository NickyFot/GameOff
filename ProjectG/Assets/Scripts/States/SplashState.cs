using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashState : GameState
{
    protected override void OnStart()
    {
        AudioManager.Instance.PlayAutomaticAudioGroup(5, DataManager.Data.AmbientGroup);
        AudioManager.Instance.SetChannelVolume(AudioManager.ChannelType.FX, 0.3f);

        AudioManager.Instance.Play2DAudio(Resources.Load<AudioClip>("Audio/Ambience/Sea Waves"), AudioManager.ChannelType.AMBIENCE, true);
        AudioManager.Instance.SetChannelVolume(AudioManager.ChannelType.AMBIENCE, 0);
        AudioManager.Instance.FadeChannel(AudioManager.ChannelType.AMBIENCE, 0.3f, 1);

        GameManager.Instance.TransitionToNewState(GameManager.Instance.State<MainMenuState>());
    }

    protected override void OnEnd()
    {
    }

    protected override void OnUpdate()
    {
    }

    protected override void OnFixedUpdate()
    {
    }
}
