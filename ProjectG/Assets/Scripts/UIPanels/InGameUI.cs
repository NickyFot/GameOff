using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class InGameUI : UIPanel
{
    private PauseMenu m_PauseMenu;
    private CountdownUI m_CountDown;

    private GameObject m_TimerPanel;
    private TextMeshProUGUI m_TurnTimerText;
    private float m_Timer;
    public float timerText
    {
        set
        {
            m_Timer = value;
            m_TurnTimerText.text = m_Timer.ToString();
        }
    }

    private int m_PlayerCount = 0;
    private List<PlayerPanel> m_PlayerHPPanels = new List<PlayerPanel>(4);
    private GameObject m_WinningPanel;
    private Button m_MainMenu;
    private Button m_NewRound;

    public InGameUI()
    {
        PanelObj = UIManager.MainCanvas.transform.Find("InGameUI").gameObject;
        if(m_CountDown == null)
        {
            GameObject temp = GameObject.Instantiate(Resources.Load<GameObject>("CountDownUI"), PanelObj.transform);
            m_CountDown = temp.GetComponent<CountdownUI>();
        }
        m_TimerPanel = PanelObj.transform.Find("TurnTimer").gameObject;
        m_TurnTimerText = m_TimerPanel.GetComponent<TextMeshProUGUI>();
        m_WinningPanel = GameObject.Instantiate(Resources.Load<GameObject>("Winner"), PanelObj.transform);

        m_MainMenu = m_WinningPanel.transform.Find("MainMenu").GetComponent<Button>();
        SetButtonMethod(m_MainMenu, MainMenuAction);
        m_NewRound = m_WinningPanel.transform.Find("NewRound").GetComponent<Button>();
        SetButtonMethod(m_NewRound, NewRoundAction);

        m_PauseMenu = new PauseMenu(PanelObj.transform.Find("PausePanel").gameObject);
    }

    //-----Winner------------------------------------------------------------
    public void ShowWinner(bool enable)
    {
        m_WinningPanel.SetActive(enable);
    }

    private void NewRoundAction()
    {
        AudioManager.Instance.Play2DAudio(p_ButtonClick, AudioManager.ChannelType.FX);
        m_WinningPanel.SetActive(false);
        FightManager.Instance.SetupNewRound();
    }

    public void MainMenuAction()
    {
        Time.timeScale = 1;
        AudioManager.Instance.Play2DAudio(p_ButtonClick, AudioManager.ChannelType.FX);

        HidePanel();
        FightManager.Instance.StopGame();
        DestroyPlayerPanels();

        GameManager.Instance.GoToMainMenu();

        //UIManager.Instance.WaitForTransitionToEnd(GoToMainMenu);
    }

    private void GoToMainMenu()
    {
        GameManager.Instance.GoToMainMenu();
    }


    //-----Timer------------------------------------------------------------

    public void StartCountDown()
    {
        m_CountDown.Reset();
        m_CountDown.Count = true;
    }

    public void ShowTurnTimer(bool enable)
    {
        m_TimerPanel.SetActive(enable);
    }

    //------Player Panels-----------------------------------------------------------

    public void CreatePanel(string playerName, Camera faceCam)
    {
        GameObject hpPrefab = Resources.Load<GameObject>("PlayerHP");
        PlayerPanel panel = new PlayerPanel(hpPrefab, this.PanelObj.transform);
        panel.SetPlayerName(playerName);
        panel.FaceCamImg.texture = faceCam.targetTexture;
        m_PlayerHPPanels.Add(panel);

        m_PlayerCount++;

        switch (m_PlayerCount)
        {
            case 1:
                panel.SetPosition(PanelPositions.Player1Center);
                break;
            case 2:
                panel.SetPosition(PanelPositions.Player2Center);
                break;
            case 3:
                panel.SetPosition(PanelPositions.Player3Center);
                break;
            case 4:
                panel.SetPosition(PanelPositions.Player4Center);
                break;
        }
    }

    public void UpdateHpFor(int playerId, float hpPercentage)
    {
        m_PlayerHPPanels[playerId].UpdateHPBar(hpPercentage);
    }

    public void SetOnCountdownEnd(Action action)
    {
        m_CountDown.OnCountdownEnd = action;
    }

    public void TogglePausePanel(Boolean IsPaused)
    {
        m_PauseMenu.Show(IsPaused);
    }

    public void DestroyPlayerPanels()
    {
        m_PlayerCount = 0;
        for(int i = 0; i < m_PlayerHPPanels.Count; i++)
        {
            GameObject.Destroy(m_PlayerHPPanels[i].m_PanelObj);
        }
        m_PlayerHPPanels.Clear();
    }


    private class PlayerPanel
    {
        public GameObject m_PanelObj;
        private Image HPbar;
        private TextMeshProUGUI PlayerName;
        private RectTransform m_RectTransform;
        public Vector2 PlayerPosition;
        public RawImage FaceCamImg { get; private set; }

        public PlayerPanel(GameObject prefab, Transform parent)
        {
            m_PanelObj = GameObject.Instantiate(prefab);
            m_PanelObj.transform.parent = parent;
            PlayerName = m_PanelObj.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            HPbar = m_PanelObj.transform.Find("Bar level").GetComponent<Image>();
            m_RectTransform = m_PanelObj.GetComponent<RectTransform>();
            FaceCamImg = m_PanelObj.transform.Find("RenderTarget").GetComponent<RawImage>();
        }

        public void SetPosition(Vector2 Position)
        {
            m_RectTransform.anchoredPosition = Position;
        }

        public void SetPlayerName(string Player_ID)
        {
            PlayerName.text = Player_ID;
        }

        public void UpdateHPBar(float hpPercentage)
        {
            HPbar.fillAmount = hpPercentage;
        }
    }

    private struct PanelPositions
    {
        public static Vector2 Player1Center = new Vector2(447 / 2, Screen.height - (128 / 2));
        public static Vector2 Player2Center = new Vector2(Screen.width - 447 / 2, Screen.height - ( 128 / 2));
        public static Vector2 Player3Center = new Vector2(447 / 2, 128 / 2);
        public static Vector2 Player4Center = new Vector2(Screen.width - 447 / 2, 128 / 2);
    }

}

