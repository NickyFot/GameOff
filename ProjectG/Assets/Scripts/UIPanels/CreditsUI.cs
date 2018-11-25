using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : UIPanel
{

    //-----------------------------------------------------------------

    private Button BackBtn;

    //-----------------------------------------------------------------
    public CreditsUI()
    {
        PanelObj = UIManager.MainCanvas.transform.Find("Credits").gameObject;

        BackBtn = PanelObj.transform.Find("BackButton").GetComponent<Button>();
        SetButtonMethod(BackBtn, BackAction);
    }
    //-----------------------------------------------------------------

    private void BackAction()
    {
        AudioManager.Instance.Play2DAudio(p_ButtonClick, AudioManager.ChannelType.FX);
        UIManager.Instance.Credits.HidePanel();
        UIManager.Instance.MainMenu.ShowPanel();
    }
}
