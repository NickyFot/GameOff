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

    //-----------------------------------------------------------------

    public Controller(int controllerNumber)
    {
        m_ControllerNumber = controllerNumber * 20;

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

    public void UpdateInput()
    {
        if(m_Unit == null) return;
        AxisX = Input.GetAxis("Horizontal");
        AxisY = Input.GetAxis("Vertical");

        if(Input.GetKeyDown((KeyCode) 350 + m_ControllerNumber)) ButtonA.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 351 + m_ControllerNumber)) ButtonB.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 352 + m_ControllerNumber)) ButtonX.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 353 + m_ControllerNumber)) ButtonY.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 354 + m_ControllerNumber)) ButtonLB.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 355 + m_ControllerNumber)) ButtonRB.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 356 + m_ControllerNumber)) ButtonBack.Execute(m_Unit);
        if(Input.GetKeyDown((KeyCode) 357 + m_ControllerNumber)) ButtonStart.Execute(m_Unit);
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
