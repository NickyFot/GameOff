﻿using System.Collections;
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
    private Dictionary<string, PlayerPanel> playerPanels = new Dictionary<string, PlayerPanel>(4);
    private GameObject m_WinningPanel;

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

        m_PauseMenu = new PauseMenu(PanelObj.transform.Find("PausePanel").gameObject);
    }

    //-----Winner------------------------------------------------------------
    public void ShowWinner(bool enable)
    {
        m_WinningPanel.SetActive(enable);
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

    public void CreatePanel(string playerName)
    {
        GameObject hpPrefab = Resources.Load<GameObject>("PlayerHP");
        PlayerPanel panel = new PlayerPanel(hpPrefab, this.PanelObj.transform);
        panel.SetPlayerName(playerName);

        playerPanels.Add(playerName, panel);

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

    public void UpdateHpFor(string playerName, float hpPercentage)
    {
        playerPanels[playerName].UpdateHPBar(hpPercentage);
    }

    public void SetOnCountdownEnd(Action action)
    {
        m_CountDown.OnCountdownEnd = action;
    }

    public void TogglePausePanel(Boolean IsPaused)
    {
        m_PauseMenu.Show(IsPaused);
    }

    private class PlayerPanel
    {
        private GameObject m_ParentObj;
        private Image HPbar;
        private TextMeshProUGUI PlayerName;
        private RectTransform m_RectTransform;
        public Vector2 PlayerPosition;

        public PlayerPanel(GameObject prefab, Transform parent)
        {
            m_ParentObj = GameObject.Instantiate(prefab);
            m_ParentObj.transform.parent = parent;
            PlayerName = m_ParentObj.transform.Find("Text").GetComponent<TextMeshProUGUI>();
            HPbar = m_ParentObj.transform.Find("Bar level").GetComponent<Image>();
            m_RectTransform = m_ParentObj.transform.Find("").GetComponent<RectTransform>();
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
        public static Vector2 Player1Center = new Vector2(370 / 2, Screen.height - (55 / 2));
        public static Vector2 Player2Center = new Vector2(Screen.width - 370 / 2, Screen.height - (55 / 2));
        public static Vector2 Player3Center = new Vector2(370 / 2, 55 / 2);
        public static Vector2 Player4Center = new Vector2(Screen.width - 370 / 2, 55 / 2);
    }

}

