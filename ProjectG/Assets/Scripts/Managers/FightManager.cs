using UnityEngine;
using System.Collections.Generic;

public class FightManager : Singleton<FightManager>
{
    //-----------------------------------------------------------------

    // -- General Vars
    private GameplayData m_Data;

    public GameObject ArenaObject;

    // -- Setup Vars
    public List<Vector3> SpawnPointList = new List<Vector3>();
    public List<Unit> AliveFightersList = new List<Unit>();

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

    private DowntimeCounter downtime;

    //-----------------------------------------------------------------

    public FightManager()
    {
        if(m_Data == null)
        {
            m_Data = Resources.Load<GameplayData>("ScriptableObjects/GameplayData");
        }
    }

    //-----------------------------------------------------------------

    // TO - DO: ADD LEVEL PARAM
    public void SetupNewRound()
    {
        m_CurrentState = TurnState.INTRO_STATE;
        m_TurnTrigger = m_Data.TurnTime;
        m_WaitForInputTrigger = m_Data.WaitForInputTime / (1 / m_Data.SlowDownScale);

     

        SpawnPointList = new List<Vector3>(GetSpawnPoints(ArenaObject));

        Unit fighter1 = new PlayerUnit("PlayerUnit", SpawnPointList[0]);
        InputManager.Instance.AssignUnitToNextController(fighter1);

        CameraManager.Instance.InitCamera(Vector3.zero);
        CameraManager.Instance.SetCameraPositionBoundaries(14, -14f, 15f, -15f);
        CameraManager.Instance.AddTarget(fighter1.UnitObj);

        Unit fighter2 = new NPCUnit("PlayerUnit", SpawnPointList[1]);
        fighter2.UnitParentObj.name = "NPC";
        CameraManager.Instance.AddTarget(fighter2.UnitObj);

        AliveFightersList.Add(fighter1);
        AliveFightersList.Add(fighter2);

        StartFight();
        

        //Unit fighter3 = new NPCUnit("PlayerUnit", SpawnPointList[1]);
        //fighter3.UnitObj.name = "NPC";

        //Debug.Log(fighter2.Equals(fighter3));

        //CameraManager.Instance.AddTarget(fighter2.UnitObj);
        //CameraManager.Instance.AddTarget(fighter3.UnitObj);
    }

    //-----------------------------------------------------------------

    public void UpdateFight()
    {
        switch(m_CurrentState)
        {
            case TurnState.INTRO_STATE:
                if (Input.GetKey(KeyCode.Space))
                {
                    GoToTimeStoppedState();
                }
                break;
            case TurnState.FIGHTING_TIME_STOPPED:
            {
              
                m_WaitForInputTimer += Time.deltaTime;
                if(m_WaitForInputTimer > m_WaitForInputTrigger)
                {
                    downtime.Count = false;
                    GoToTimeRunningState();
                }
                else
                {
                    downtime.Count = true;
                    downtime.timerTrgger = m_WaitForInputTimer;
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
                break;
        }


    }

    //-----------------------------------------------------------------

    private void StartFight()
    {
        UIManager.Instance.GameUI.ShowPanel();
        UIManager.Instance.StartCountDown();
    }

    private void GoToTimeStoppedState()
    {
        m_WaitForInputTimer = 0;
        m_CurrentState = TurnState.FIGHTING_TIME_STOPPED;
        Time.timeScale = m_Data.SlowDownScale;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        InputManager.Instance.InputEnabled = true;
        Debug.Log("MOVING TO FIGHT_STOP");
    }

    private void GoToTimeRunningState()
    {
        m_TurnTimer = 0;
        m_CurrentState = TurnState.FIGHTING_TIME_RUNNING;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02F;
        InputManager.Instance.InputEnabled = false;
        Debug.Log("MOVING TO FIGHT_RUN");
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
}
