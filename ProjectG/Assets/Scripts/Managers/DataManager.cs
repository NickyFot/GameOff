using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    private static GameplayData m_DataManager;
    public static GameplayData Data
    {
        get
        {
            if(m_DataManager == null)
            {
                m_DataManager = Resources.Load<GameplayData>("ScriptableObjects/GameplayData");
            }
            return m_DataManager;
        }
    }

}
