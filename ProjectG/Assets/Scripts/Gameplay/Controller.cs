using UnityEngine;
using System.Collections;

public class Controller
{
    //-----------------------------------------------------------------

    public float AxisX;
    public float AxisY;

    public Command ButtonA;
    public Command ButtonB;
    public Command ButtonX;
    public Command ButtonY;
    public Command ButtonLB;
    public Command ButtonRB;
    public Command ButtonBack;
    public Command ButtonStart;

    public bool ActivePlayer;

    private Unit m_Unit;
    private int m_ControllerNumber;

    //-----------------------------------------------------------------

    public Controller(int controllerNumber)
    {
        m_ControllerNumber = controllerNumber * 20;

        ButtonA = new Command();
        ButtonB = new Command();
        ButtonX = new Command();
        ButtonY = new Command();
        ButtonLB = new Command();
        ButtonRB = new Command();
        ButtonBack = new Command();
        ButtonStart = new Command();

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
