using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : UIPanel
{
    //-----------------------------------------------------------------

    private Button m_PlayBtn;
    private Button m_CreditsBtn;
    private Button m_ExitBtn;

    //-----------------------------------------------------------------

    public MainMenuUI()
    {
       PanelObj = UIManager.MainCanvas.transform.Find("MainMenu").gameObject;

        m_PlayBtn = PanelObj.transform.Find("PlayButton").GetComponent<Button>();
        SetButtonMethod(m_PlayBtn, PlayAction);

        m_CreditsBtn = PanelObj.transform.Find("CreditsButton").GetComponent<Button>();
        SetButtonMethod(m_CreditsBtn, CreditsAction);

        m_ExitBtn = PanelObj.transform.Find("ExitButton").GetComponent<Button>();
        SetButtonMethod(m_ExitBtn, ExitAction);

    }

    //-----------------------------------------------------------------

    private void PlayAction()
    {
        //Debug.Log("Pressed Options");
        AudioManager.Instance.Play2DAudio(p_ButtonClick, AudioManager.ChannelType.FX);
        UIManager.Instance.MainMenu.HidePanel();
        UIManager.Instance.PlaySettings.ShowPanel();
        //UIManager.Instance.WaitForTransitionToEnd(TransitionIntoGame);
    }

    //private void TransitionIntoGame()
    //{
    //    ArenaManager.Instance.TransitionToArena();
    //}

    private void CreditsAction()
    {
        //Debug.Log("Pressed Options");
        AudioManager.Instance.Play2DAudio(p_ButtonClick, AudioManager.ChannelType.FX);
        UIManager.Instance.MainMenu.HidePanel();
        UIManager.Instance.Credits.ShowPanel();
    }

    private void ExitAction()
    {
        AudioManager.Instance.Play2DAudio(p_ButtonClick, AudioManager.ChannelType.FX);
        Application.Quit();
    }

}
