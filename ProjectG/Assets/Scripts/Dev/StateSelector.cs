using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateSelector : MonoBehaviour
{
    public enum States
    {
        Splash,
        MainMenu,
        InGame,
    }

    public States StartingState;

    public void Start()
    {
        GameState state = null;
        switch(StartingState)
        {
            case States.Splash:
                state = GameManager.Instance.State<SplashState>();
                break;
            case States.MainMenu:
                state = GameManager.Instance.State<MainMenuState>();
                break;
            case States.InGame:
                ArenaManager.Instance.TransitionToArena();
                //state = GameManager.Instance.State<InGameState>();
                break;
        }

        if(state != null)
        {
            GameManager.Instance.TransitionToNewState(state);
        }
    }
}
