using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameState : GameState
{
    protected override void OnStart()
    {
        FightManager.Instance.ArenaObject = GameObject.FindGameObjectWithTag("Arena");
        FightManager.Instance.SetupNewRound();

        AudioManager.Instance.CrossfadeTo(AudioManager.ChannelType.MUSIC, Resources.Load<AudioClip>("Audio/Reuben_s_Train"), 1, 0, true);
        AudioManager.Instance.SetChannelVolume(AudioManager.ChannelType.MUSIC, 0);
        AudioManager.Instance.FadeChannel(AudioManager.ChannelType.MUSIC, 1, 2);
    }

    protected override void OnEnd()
    {
        AudioManager.Instance.StopChannel(AudioManager.ChannelType.MUSIC);
    }

    protected override void OnUpdate()
    {
        FightManager.Instance.UpdateFight();
    }

    protected override void OnFixedUpdate()
    {
        CameraManager.Instance.UpdateCamera();
    }
}
