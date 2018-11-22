using System;
using UnityEngine;
using UnityEngine.UI;

public class CountdownUI : MonoBehaviour
{
    public GameObject[] States;
    public float timerTrgger;

    private float m_TimerCounter;
    private int m_CurrentState;

    public bool Count { get; set; }

    void Start ()
    {
        Reset();
	}
	
	void Update ()
    {
        if(Count)
        {
            m_TimerCounter += Time.deltaTime;
            if (m_TimerCounter > timerTrgger)
            {
                GoToNextState();
                m_TimerCounter = 0;
            }
        }
    }

    public void Reset()
    {
        m_CurrentState = 0;
        m_TimerCounter = 0;
    }

    public void GoToNextState()
    {
        //Debug.Log(m_CurrentState);
        int max_index = States.Length;
        if(m_CurrentState == 0)
        {
            States[m_CurrentState].SetActive(true);
            m_CurrentState++;
        }
        else if(m_CurrentState < max_index)
        {
            States[m_CurrentState - 1].SetActive(false);
            States[m_CurrentState].SetActive(true);
            m_CurrentState++;
        }
        else
        {
            States[m_CurrentState - 1].SetActive(false);
            Count = false;
        }
    }
}
