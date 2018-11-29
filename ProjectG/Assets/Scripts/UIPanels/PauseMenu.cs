using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class PauseMenu : UIPanel {

    //private GameObject Prefab = Resources.Load<GameObject>("PausePanel");
    private GameObject m_ParentObject;
    private TextMeshProUGUI PauseText;
    private Button ResumeButton;
    private Button ExitButton;

    public PauseMenu(GameObject PausePanel)
    {
        //m_ParentObj = GameObject.Instantiate(Prefab);
        m_ParentObject = PausePanel.gameObject;
        
        ResumeButton = m_ParentObject.transform.Find("ResumeButton").GetComponent<Button>();
        ExitButton = m_ParentObject.transform.Find("ExitButton").GetComponent<Button>();
        PauseText = m_ParentObject.transform.Find("PauseText").GetComponent<TextMeshProUGUI>();
        Show(false);
    }

    public void Show(Boolean IsPaused)
    {
        m_ParentObject.SetActive(IsPaused);
    }
}
