using System;
using UnityEngine;
using UnityEngine.UI;

public class PlaySettingsUI : UIPanel
{
    //-----------------------------------------------------------------

    private Button m_PlayBtn;
    private Button m_BackBtn;
    private Button m_AddPlayer;
    private Button m_RemovePlayer;

    private Text m_NumbPlayers;

    private int m_Counter;

    public int Counter { get { return m_Counter; } }

    //-----------------------------------------------------------------
    public PlaySettingsUI()
    {
        m_Counter = 2;
        PanelObj = UIManager.MainCanvas.transform.Find("PlaySettings").gameObject;
        m_NumbPlayers = PanelObj.transform.Find("number of players").GetComponent<Text>();

        m_PlayBtn = PanelObj.transform.Find("PlayButton").GetComponent<Button>();
        SetButtonMethod(m_PlayBtn, PlayAction);

        m_BackBtn = PanelObj.transform.Find("BackButton").GetComponent<Button>();
        SetButtonMethod(m_BackBtn, BackAction);

        m_AddPlayer = PanelObj.transform.Find("Add").GetComponent<Button>();
        SetButtonMethod(m_AddPlayer, AddPlayer);

        m_RemovePlayer = PanelObj.transform.Find("Remove").GetComponent<Button>();
        SetButtonMethod(m_RemovePlayer, RemovePlayer);
    }
    //-----------------------------------------------------------------

    private void AddPlayer()
    {
        if(m_Counter < 4)
        {
            m_Counter++;
            m_NumbPlayers.text = m_Counter.ToString();
        }
    }

    private void RemovePlayer()
    {
        if (m_Counter > 2)
        {
            m_Counter--;
            m_NumbPlayers.text = m_Counter.ToString();
        }
    }

    private void PlayAction()
    {
        AudioManager.Instance.Play2DAudio(p_ButtonClick, AudioManager.ChannelType.FX);
        UIManager.Instance.PlaySettings.HidePanel();
        UIManager.Instance.WaitForTransitionToEnd(TransitionIntoGame);
    }

    private void TransitionIntoGame()
    {
        ArenaManager.Instance.TransitionToArena();
    }

    private void BackAction()
    {
        AudioManager.Instance.Play2DAudio(p_ButtonClick, AudioManager.ChannelType.FX);
        UIManager.Instance.PlaySettings.HidePanel();
        UIManager.Instance.MainMenu.ShowPanel();
    }
}
