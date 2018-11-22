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
        SetupUI();
    }
    //-----------------------------------------------------------------

    public override void SetupUI()
    {
        PanelObj = UIManager.MainCanvas.transform.Find("Credits").gameObject;

        BackBtn = PanelObj.transform.Find("BackButton").GetComponent<Button>();
        SetButtonMethod(BackBtn, BackAction);
    }
    //-----------------------------------------------------------------

    private void BackAction()
    {
        UIManager.Instance.Credits.HidePanel();
        UIManager.Instance.MainMenu.ShowPanel();
    }
}
