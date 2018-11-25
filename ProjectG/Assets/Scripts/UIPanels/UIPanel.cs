using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class UIPanel
{
    //-----------------------------------------------------------------

    public GameObject PanelObj;

    private static AudioClip m_ButtonClick;
    protected static AudioClip p_ButtonClick
    {
        get
        {
            if(m_ButtonClick == null)
            {
                m_ButtonClick = Resources.Load<AudioClip>("Audio/Other/ButtonClick");
            }
            return m_ButtonClick;
        }
    }

    public virtual void ShowPanel()
    {
        UIManager.Instance.FadeInUI(PanelObj, 0.6f, true);
    }

    public virtual void HidePanel()
    {
        UIManager.Instance.FadeOutUI(PanelObj, 0.8f, true);
    }

    //-----------------------------------------------------------------

    protected void SetButtonMethod( Button button, UnityAction action )
    {
        //Remove the existing events
        button.onClick.RemoveAllListeners();
        //Add your new event
        button.onClick.AddListener(action);
    }

    protected void SetToggleBool( Toggle toggle, UnityAction<bool> isTrue )
    {
        //Remove the existing events
        toggle.onValueChanged.RemoveAllListeners();
        //Add your new event
        toggle.onValueChanged.AddListener(isTrue);
    }

    //-----------------------------------------------------------------
}
