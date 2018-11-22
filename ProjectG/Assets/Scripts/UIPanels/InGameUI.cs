using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class InGameUI : UIPanel
{
    private CountdownUI m_CountDown;
    private GameObject m_HPPrefab;
    
    private int m_PlayerCount = 0;

    public InGameUI()
    {
        SetupUI();
    }

    public override void SetupUI()
    {
        PanelObj = UIManager.MainCanvas.transform.Find("InGameUI").gameObject;
        m_HPPrefab = Resources.Load<GameObject>("PlayerHP");
        if(m_CountDown == null)
        {
            GameObject temp = GameObject.Instantiate(Resources.Load<GameObject>("CountDownUI"), PanelObj.transform);
            m_CountDown = temp.GetComponent<CountdownUI>();
        }
    }

    public void CreatePanel(string playerName)
    {
        PlayerPanel Panel = new PlayerPanel(m_HPPrefab, this.PanelObj.transform);

        Panel.SetPlayerName(playerName);

        m_PlayerCount++;

        switch (m_PlayerCount)
        {
            case 1:
                Panel.SetPosition(PanelPositions.Player1Center);
                break;
            case 2:
                Panel.SetPosition(PanelPositions.Player2Center);
                break;
            case 3:
                Panel.SetPosition(PanelPositions.Player3Center);
                break;
            case 4:
                Panel.SetPosition(PanelPositions.Player4Center);
                break;
        }

    }

    public void StartCountDown()
    {
        m_CountDown.Reset();
        m_CountDown.Count = true;
    }

    private class PlayerPanel
    {
        private GameObject m_ParentObj;
        private Image HPbar;
        private TMPro.TextMeshProUGUI PlayerName;
        private RectTransform m_RectTransform ;
        public Vector2 PlayerPosition;

        public PlayerPanel(GameObject prefab, Transform parent)
        {
            m_ParentObj = GameObject.Instantiate(prefab);
            m_ParentObj.transform.parent = parent;
            PlayerName = m_ParentObj.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
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

        private void UpdateHPBar(float hpPercentage)
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

