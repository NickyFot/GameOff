using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : UIPanel
{
    //-----------------------------------------------------------------

    private Button PlayBtn;
    private Button CreditsBtn;
    private Button ExitBtn;

    //-----------------------------------------------------------------

    public MainMenuUI()
    {
        SetupUI();
    }

    //-----------------------------------------------------------------

    public override void SetupUI()
    {
        PanelObj = UIManager.MainCanvas.transform.Find("MainMenu").gameObject;

        PlayBtn = PanelObj.transform.Find("PlayButton").GetComponent<Button>();
        SetButtonMethod(PlayBtn, PlayAction);

        CreditsBtn = PanelObj.transform.Find("CreditsButton").GetComponent<Button>();
        SetButtonMethod(CreditsBtn, CreditsAction);

        ExitBtn = PanelObj.transform.Find("ExitButton").GetComponent<Button>();
        SetButtonMethod(ExitBtn, ExitAction);
    }

    //-----------------------------------------------------------------

    private void PlayAction()
    {
        Debug.Log("Pressed Options");

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
        Debug.Log("Pressed Options");
        UIManager.Instance.MainMenu.HidePanel();
        UIManager.Instance.Credits.ShowPanel();
    }

    private void ExitAction()
    {
        Application.Quit();
    }

    //-----------------------------------------------------------------

    public override void ShowPanel()
    {
        base.ShowPanel();
        AudioManager.Instance.Play2DAudio(Resources.Load<AudioClip>("Audio/Blow_The_Man_Down"), AudioManager.ChannelType.MUSIC, true);
        AudioManager.Instance.SetChannelVolume(AudioManager.ChannelType.MUSIC, 0);
        AudioManager.Instance.FadeChannel(AudioManager.ChannelType.MUSIC, 1, 2);
    }

    public override void HidePanel()
    {
        base.HidePanel();
        AudioManager.Instance.FadeChannel(AudioManager.ChannelType.MUSIC, 0, 1.5f, delegate { AudioManager.Instance.StopChannel(AudioManager.ChannelType.MUSIC); });
    }

    //-----------------------------------------------------------------
}
