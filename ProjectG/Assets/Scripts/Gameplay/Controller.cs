using UnityEngine;
using System.Collections;

public class Controller
{
    //-----------------------------------------------------------------

    public float AxisX;
    public float AxisY;

    public InputCommand ButtonA;
    public InputCommand ButtonB;
    public InputCommand ButtonX;
    public InputCommand ButtonY;
    public InputCommand ButtonLB;
    public InputCommand ButtonRB;
    public InputCommand ButtonBack;
    public InputCommand ButtonStart;

    public bool ActivePlayer;

    private Unit m_Unit;
    private int m_ControllerNumber;
    private int m_ControllerMod;


    //-----------------------------------------------------------------

    public Controller(int controllerNumber)
    {
        m_ControllerNumber = controllerNumber;
        m_ControllerMod = controllerNumber * 20;

        ButtonA = new InputCommand();
        ButtonB = new InputCommand();
        ButtonX = new InputCommand();
        ButtonY = new InputCommand();
        ButtonLB = new InputCommand();
        ButtonRB = new InputCommand();
        ButtonBack = new InputCommand();
        ButtonStart = new InputCommand();

        ActivePlayer = false;
    }

    //-----------------------------------------------------------------

    public void UpdateCommandInput()
    {
        if(m_Unit == null) return;
        if(Input.GetKeyDown((KeyCode) 350 + m_ControllerMod)) ButtonA.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 351 + m_ControllerMod)) ButtonB.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 352 + m_ControllerMod)) ButtonX.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 353 + m_ControllerMod)) ButtonY.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 354 + m_ControllerMod)) ButtonLB.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 355 + m_ControllerMod)) ButtonRB.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 356 + m_ControllerMod)) ButtonBack.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 357 + m_ControllerMod)) ButtonStart.Execute(m_Unit);
    }

    public void UpdateMoveInput()
    {
        if(m_Unit == null) return;
        AxisX = Input.GetAxis("Horizontal" + m_ControllerNumber);
        AxisY = Input.GetAxis("Vertical" + m_ControllerNumber);
    }

    //-----------------------------------------------------------------

    public void SetUnit(Unit u)
    {
        m_Unit = u;
    }

    public Unit GetUnit()
    {
        return m_Unit;
    }

    //-----------------------------------------------------------------
}
