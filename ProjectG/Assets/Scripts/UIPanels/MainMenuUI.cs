using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : UIPanel
{
    //-----------------------------------------------------------------

    private Button PlayBtn;
    private Button OptionsBtn;
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

        PlayBtn = PanelObj.transform.Find("PlayBtn").GetComponent<Button>();
        SetButtonMethod(PlayBtn, PlayAction);

        OptionsBtn = PanelObj.transform.Find("OptionsBtn").GetComponent<Button>();
        SetButtonMethod(OptionsBtn, OptionsAction);

        ExitBtn = PanelObj.transform.Find("ExitBtn").GetComponent<Button>();
        SetButtonMethod(ExitBtn, ExitAction);
    }

    //-----------------------------------------------------------------

    private void PlayAction()
    {
        UIManager.Instance.MainMenu.HidePanel();
        UIManager.Instance.WaitForTransitionToEnd(TransitionIntoGame);
    }

    private void TransitionIntoGame()
    {
        ArenaManager.Instance.TransitionToArena();
    }

    private void OptionsAction()
    {
        Debug.Log("Pressed Options");
    }

    private void ExitAction()
    {
        Application.Quit();
    }

    //-----------------------------------------------------------------
}
