using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;


public class PauseMenu : UIPanel {

    //private GameObject Prefab = Resources.Load<GameObject>("PausePanel");
    private TextMeshProUGUI PauseText;
    public Button ResumeButton;
    private Button ExitButton;

    public PauseMenu(GameObject PausePanel)
    {
        //m_ParentObj = GameObject.Instantiate(Prefab);
        PanelObj = PausePanel.gameObject;
        
        ResumeButton = PanelObj.transform.Find("ResumeButton").GetComponent<Button>();
        SetButtonMethod(ResumeButton, OnResume);

        ExitButton = PanelObj.transform.Find("ExitButton").GetComponent<Button>();
        SetButtonMethod(ExitButton, MainMenuAction);

        PauseText = PanelObj.transform.Find("PauseText").GetComponent<TextMeshProUGUI>();
        Show(false);
    }

    public void Show(Boolean IsPaused)
    {
        PanelObj.SetActive(IsPaused);
    }

    private void MainMenuAction()
    {
        PanelObj.SetActive(false);
        UIManager.Instance.GameUI.MainMenuAction();
    }

    private void OnResume()
    {
        FightManager.Instance.TogglePause();
    }
}
