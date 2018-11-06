using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameState : GameState
{
    protected override void OnStart()
    {
        FightManager.Instance.ArenaObject = GameObject.FindGameObjectWithTag("Arena");
        FightManager.Instance.SetupNewRound();
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
