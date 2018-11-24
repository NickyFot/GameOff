using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DowntimeCounter : MonoBehaviour
{

    public float timerTrgger { get; set; }
    public bool Count { get; set; }

    public GameObject InGameUIPanel;

    private TextMeshPro m_text;
    // Use this for initialization
    void Start()
    {
        m_text = InGameUIPanel.GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Count)
        {
            InGameUIPanel.SetActive(true);
            m_text.text = timerTrgger.ToString();
        }
        else
        {
            InGameUIPanel.SetActive(false);
        }
        
    }
}
