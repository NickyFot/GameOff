using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    //-----------------------------------------------------------------

    #region Singleton

    private static GameManager s_Instance;
    public static GameManager Instance
    {
        get { return s_Instance; }
    }

    #endregion

    //-----------------------------------------------------------------

    #region Game States

    private readonly TypeDictionary<GameState> m_StateDictionary = new TypeDictionary<GameState>();

    public T State<T>() where T : GameState
    {
        return m_StateDictionary.Get<T>() as T;
    }
    public GameState CurrentState
    {
        get; private set;
    }

    #endregion

    #region Manager Instances

    private UIManager UI;
    private FightManager Fight;
    private InputManager InputM;
    private CameraManager CameraM;
    private AudioManager AudioM;

    #endregion

    //-----------------------------------------------------------------

    public void Awake()
    {
        s_Instance = this;
        DontDestroyOnLoad(this);

        m_StateDictionary.Add<SplashState>();
        m_StateDictionary.Add<MainMenuState>();
        m_StateDictionary.Add<InGameState>();
        m_StateDictionary.Add<TransitionState>();
    }

    public void Start()
    {
        UI = UIManager.Instance;
        InputM = InputManager.Instance;
        CameraM = CameraManager.Instance;
        Fight = FightManager.Instance;
        AudioM = AudioManager.Instance;
        AudioM.Init();
        DontDestroyOnLoad(AudioM.GlobalSourceObj);

        CurrentState = null;
        //TransitionToNewState(State<MainMenuState>());
    }

    //-----------------------------------------------------------------

    public void Update()
    {
        UI.UpdateFade();
        UI.UpdateUIAnimations();
        UI.Update();

        InputM.Update();

        Fight.UpdateFight();

        AudioM.Update();

        if(CurrentState == null) return;
        CurrentState.Update();
    }

    public void FixedUpdate()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    CameraM.dzEnabled = true;
        //    Debug.Log("wiiiiiii");
        //}
        CameraM.UpdateCamera();
        if (CurrentState == null) return;
        CurrentState.FixedUpdate();
    }

    public void LateUpdate()
    {

    }

    //-----------------------------------------------------------------

    public void TransitionToNewState(GameState newState)
    {
        if(CurrentState != null)
        {
            CurrentState.End();
        }
        CurrentState = newState;
        CurrentState.Start();
    }

    public bool AlreadyContains<T>() where T : GameState
    {
        return m_StateDictionary.Contains<T>();
    }

    //-----------------------------------------------------------------
}
