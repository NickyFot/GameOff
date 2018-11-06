﻿using UnityEngine;
using System.Collections.Generic;

public class InputManager : Singleton<InputManager>
{
    //-----------------------------------------------------------------

    public const int MAX_PLAYERS = 4;

    private List<Controller> m_ControllerList = new List<Controller>();
    private List<Controller> m_ActiveControllerList = new List<Controller>();

    //-----------------------------------------------------------------

    public InputManager()
    {  
        for(int i = 0; i < MAX_PLAYERS; i++)
        {
            Controller controller = new Controller(i);
            m_ControllerList.Add(controller);
            SetDefaultKeybindings(controller);
        }
    }

    public void Update()
    {
        for(int i = 0; i < m_ActiveControllerList.Count; i++)
        {
            if(m_ActiveControllerList[i].GetUnit() == null) continue;
            m_ActiveControllerList[i].UpdateInput();
            MoveCommand(m_ActiveControllerList[i].GetUnit(), m_ActiveControllerList[i].AxisX, m_ActiveControllerList[i].AxisY);
        }

#if UNITY_EDITOR
        DebugUpdate(0);
#endif

    }

    //-----------------------------------------------------------------

    #region Keybinding Functions

    public void SetDefaultKeybindings(Controller controller)
    {
        controller.ButtonRB.Execute = AttackRightArmCommand;
		controller.ButtonLB.Execute = AttackLeftArmCommand;
    }

    #endregion

    //-----------------------------------------------------------------

    #region Button Commands

    private void MoveCommand(Unit u, float x, float y)
    {
        u.MoveCommand(x, y);
    }

    private void AttackRightArmCommand(Unit u)
    {
        u.AttackCommand(UnitLimb.LimbType.RIGHT_ARM);
    }

    private void AttackLeftArmCommand(Unit u)
    {
        u.AttackCommand(UnitLimb.LimbType.LEFT_ARM);
    }

    private void BlockCommand(Unit u)
    {
        Debug.Log("I BLOCKZ");
    }

    #endregion

    //-----------------------------------------------------------------

    #region Controller Functions

    public void AssignUnitToNextController(Unit unit)
    {
        int playerNumber = m_ActiveControllerList.Count;
        if(playerNumber >= MAX_PLAYERS) return;
        m_ControllerList[playerNumber].ActivePlayer = true;
        m_ControllerList[playerNumber].SetUnit(unit);
        m_ActiveControllerList.Add(m_ControllerList[playerNumber]);
    }

    public void RemoveController(int playerNumber)
    {
        m_ControllerList[playerNumber].ActivePlayer = false;
        m_ActiveControllerList.Remove(m_ControllerList[playerNumber]);
    }

    #endregion

    //-----------------------------------------------------------------

#if UNITY_EDITOR

    #region Debug

    public void DebugUpdate(int playerIndex)
    {
        if (m_ActiveControllerList == null || m_ActiveControllerList.Count <= 0) return;
        Unit u = m_ActiveControllerList[playerIndex].GetUnit();

        float x = 0;
        float y = 0;

        if (Input.GetKey(KeyCode.W))
        {
            y = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            y = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            x = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            x = 1;
        }
        MoveCommand(u, x, y);

        //if(Input.GetMouseButtonDown(0))
        //{
        //    AttackLeftArmCommand(u);
        //}
        if(Input.GetMouseButtonDown(1))
        {
            AttackRightArmCommand(u);
        }
    }

    #endregion

#endif

    //-----------------------------------------------------------------
}