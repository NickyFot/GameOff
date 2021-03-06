﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayData", menuName = "Data/Gameplay", order = 1)]
public class GameplayData : ScriptableObject
{
    [Header("Round Gameplay Data")]
    public float SlowDownScale;

    public float WaitForInputTime;
    public float TurnTime;

    [Header("Audio")]
    public AudioClip MainMenuMusic;

    public AudioClip[] AmbientGroup;
    public AudioClip[] HitGroup;
    public AudioManager.AudioGroup ActualHitGroup;
    public AudioClip[] GruntGroup;
    public AudioManager.AudioGroup ActualGruntGroup;

    [Header("Players")]
    public string[] CharacterNames;

}