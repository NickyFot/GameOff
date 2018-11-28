using UnityEngine;
using System.Collections.Generic;

public class FightManager : Singleton<FightManager>
{
    //-----------------------------------------------------------------

    // -- General Vars
    //private GameplayData DataManager;

    public GameObject ArenaObject;

    // -- Setup Vars
    public List<Vector3> SpawnPointList = new List<Vector3>();
    public List<Unit> AliveFightersList = new List<Unit>();
    public List<Unit> ActiveFightersList = new List<Unit>();

    // -- Gameplay Vars
    private enum TurnState
    {
        INTRO_STATE,
        FIGHTING_TIME_STOPPED,
        FIGHTING_TIME_RUNNING,
        END_STATE
    }
    private TurnState m_CurrentState;

    private float m_WaitForInputTrigger;
    private float m_WaitForInputTimer;
    private float m_TurnTrigger;
    private float m_TurnTimer;

    private bool IsPaused = false;
    //-----------------------------------------------------------------

    public FightManager()
    {
        //if(DataManager == null)
        //{
        //    DataManager = Resources.Load<GameplayData>("ScriptableObjects/GameplayData");
        //}

        UIManager.Instance.GameUI.OnCountdownEnd = StartFight;
    }

    //-----------------------------------------------------------------

    public void SetupNewRound()
    {
        InputManager.Instance.InputEnabled = true;

        m_CurrentState = TurnState.INTRO_STATE;
        m_TurnTrigger = DataManager.Data.TurnTime;
        m_WaitForInputTrigger = DataManager.Data.WaitForInputTime / (1 / DataManager.Data.SlowDownScale);

        int numberPlayers = UIManager.Instance.PlaySettings.Counter;

        if(SpawnPointList.Count == 0)
        {
            SpawnPointList = new List<Vector3>(GetSpawnPoints(ArenaObject));
        }

        CameraManager.Instance.InitCamera(new Vector3(0f, 6f, -6f));
        CameraManager.Instance.SetCameraPositionBoundaries(7.168935f, -7.168935f, 7.168935f, -7.168935f);

        if(ActiveFightersList.Count == 0)
        {
            for(int i = 0; i < numberPlayers; i++)
            {
                Unit fighter = new PlayerUnit("SharkDude", DataManager.Data.CharacterNames[i], i, SpawnPointList[i % 2]);
                InputManager.Instance.AssignUnitToNextController(fighter);
                AliveFightersList.Add(fighter);
                ActiveFightersList.Add(fighter);
                UIManager.Instance.GameUI.CreatePanel(DataManager.Data.CharacterNames[i]);

                // moved here for simplicity
                fighter.OnTakeDamage = () => { UIManager.Instance.GameUI.UpdateHpFor(fighter.Name, fighter.HealthPercentage()); };
                fighter.OnDeath = () => Die(fighter);
            }
        }

        for(int i = 0; i < ActiveFightersList.Count; i++)
        {
            CameraManager.Instance.AddTarget(ActiveFightersList[i].UnitObj);
            ActiveFightersList[i].ResetHealth();
            ActiveFightersList[i].ResetQueue();
        }
        UIManager.Instance.GameUI.ShowPanel();
        UIManager.Instance.StartCountDown();
    }

    //-----------------------------------------------------------------

    public void UpdateFight()
    {
        for(int i = 0; i < AliveFightersList.Count; i++)
        {
            AliveFightersList[i].Update();
        }

        switch(m_CurrentState)
        {
            case TurnState.INTRO_STATE:
                // Wait
                break;
            case TurnState.FIGHTING_TIME_STOPPED:
            {
                m_WaitForInputTimer += Time.deltaTime;
                if(m_WaitForInputTimer > m_WaitForInputTrigger)
                {
                    GoToTimeRunningState();
                }
                else
                {
                    UIManager.Instance.GameUI.timerText = Mathf.RoundToInt((m_WaitForInputTrigger - m_WaitForInputTimer) * (1 / DataManager.Data.SlowDownScale));
                }

                for(int i = 0; i < AliveFightersList.Count; i++)
                {
                    AliveFightersList[i].Update();
                }
                break;
            }
            case TurnState.FIGHTING_TIME_RUNNING:
            {
                m_TurnTimer += Time.deltaTime;
                if(m_TurnTimer > m_TurnTrigger)
                {
                    GoToTimeStoppedState();
                }

                break;
            }
            case TurnState.END_STATE:
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    GoToIntroState();
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                Time.timeScale = 1;
                IsPaused = false;
            }
            else
            {
                Time.timeScale = 0;
                IsPaused = true;
            }

        }

    }

    //-----------------------------------------------------------------

    private void StartFight()
    {
        InputManager.Instance.InputEnabled = true;
        GoToTimeStoppedState();
    }

    private void GoToIntroState()
    {
        SetupNewRound();
    }

    private void GoToTimeStoppedState()
    {
        m_WaitForInputTimer = 0;
        m_CurrentState = TurnState.FIGHTING_TIME_STOPPED;
        Time.timeScale = DataManager.Data.SlowDownScale;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        InputManager.Instance.InputEnabled = true;
        UIManager.Instance.GameUI.ShowTurnTimer(true);
        Debug.Log("MOVING TO FIGHT_STOP");
    }

    private void GoToTimeRunningState()
    {
        m_TurnTimer = 0;
        m_CurrentState = TurnState.FIGHTING_TIME_RUNNING;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
        InputManager.Instance.InputEnabled = false;
        UIManager.Instance.GameUI.ShowTurnTimer(false);
        Debug.Log("MOVING TO FIGHT_RUN");
    }

    private void GoToEndState()
    {
        m_CurrentState = TurnState.END_STATE;
    }

    //-----------------------------------------------------------------

    private List<Vector3> GetSpawnPoints(GameObject Level)
    {
        Transform parent = Level.transform.Find("SpawnPoints");
        List<Vector3> spawnList = new List<Vector3>();
        for(int i = 0; i < parent.childCount; i++)
        {
            spawnList.Add(parent.GetChild(i).position);
        }
        return spawnList;
    }

    //-----------------------------------------------------------------

    private void Die(Unit fighter)
    {
        AliveFightersList.Remove(fighter);
        CameraManager.Instance.RemoveTarget(fighter.UnitObj);
        Debug.Log("DEATH!!");
        if(AliveFightersList.Count == 1)
        {
            GoToEndState();
        }
    }

    //-----------------------------------------------------------------
}
