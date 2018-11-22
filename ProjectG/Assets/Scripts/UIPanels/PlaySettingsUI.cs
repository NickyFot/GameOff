using System;
using UnityEngine;
using UnityEngine.UI;

public class PlaySettingsUI : UIPanel
{
    //-----------------------------------------------------------------

    private Button PlayBtn;
    private Button BackBtn;

    //-----------------------------------------------------------------
    public PlaySettingsUI()
    {
        SetupUI();
    }
    //-----------------------------------------------------------------

    public override void SetupUI()
    {
        PanelObj = UIManager.MainCanvas.transform.Find("PlaySettings").gameObject;

        PlayBtn = PanelObj.transform.Find("PlayButton").GetComponent<Button>();
        SetButtonMethod(PlayBtn, PlayAction);

        BackBtn = PanelObj.transform.Find("BackButton").GetComponent<Button>();
        SetButtonMethod(BackBtn, BackAction);
    }
    //-----------------------------------------------------------------
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
