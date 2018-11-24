using System.Collections;
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
}