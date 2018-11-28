using UnityEngine;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using RootMotion.Dynamics;

using Limb = UnitLimb.LimbType;
using Object = UnityEngine.Object;

public abstract class Unit
{
    //-- VARIABLES --------------------------------------------------------------
    // -- Data
    public UnitData Data;
    public string Name {
        get {
            return Data.Name;
        }
    }

    // -- Object
    public GameObject UnitParentObj;
    public GameObject UnitObj;

    protected SkinnedMeshRenderer[] p_SkinnedMeshes;

    public bool IsDead
    {
        get { return Data.Health == 0; }
    }

    // -- Animators
    protected Animator p_UnitAnimator;
    protected FullBodyBipedIK p_BodyIK;
    protected PuppetMaster p_Puppet;

    public Action OnTakeDamage = delegate { };
    public Action OnDeath = delegate { };

    // -- Gameplay Vars
    public UnitIdentifierMono UId { get; private set; }

    // To-DO: Expose a scriptable object or something (conect to Unit Data)
    private float m_RotationSpeed = 0.2f;

    public bool HasFinishedTurn
    {
        get
        {
            if(p_CommandQueue == null) return false;
            return p_CommandQueue.Count >= Data.MaxQueueInput;
        }
    }
    protected int p_CommandsQueued
    {
        get
        {
            if(p_CommandQueue == null) return 0;
            return p_CommandQueue.Count;
        }
    }

    protected Queue<QueuedCommand> p_CommandQueue = new Queue<QueuedCommand>();
    private float m_QueueTimer;
    private float m_QueueTrigger;

    public bool IsAttacking { get; private set; }
    private float m_AttackTrigger;
    private float m_AttackTimer;

    private bool m_IsInvulnerable;
    private float m_InvulnerableTrigger = 1.5f;
    private float m_InvulnerableTimer;

    private Limb m_LastAttackedLimb;

    // -- Movement Vars
    private float m_CurrentRotAngle;
    private float m_TargetRotAngle;
    private float m_TargetRotVel;

    // -- IK Vars

    //-- CONSTRUCTOR -------------------------------------------------------------

    public Unit(string prefabName, string name, int unitID)
    {
        UnitParentObj = Object.Instantiate(Resources.Load<GameObject>(prefabName));
        p_UnitAnimator = UnitParentObj.GetComponentInChildren<Animator>();
        UnitObj = p_UnitAnimator.gameObject;
        p_BodyIK = UnitObj.GetComponent<FullBodyBipedIK>();
        p_Puppet = UnitParentObj.GetComponentInChildren<PuppetMaster>();
        Data = new UnitData(name);
        UId = UnitParentObj.GetComponent<UnitIdentifierMono>();
        UId.UnitID = unitID;
        UId.UnitRef = this;

        p_SkinnedMeshes = UnitParentObj.GetComponentsInChildren<SkinnedMeshRenderer>();
        Material mat = UnitData.GetMaterial(unitID);

        for(int i = 0; i < p_SkinnedMeshes.Length; i++)
        {
            p_SkinnedMeshes[i].material = mat;
        }
    }

    //-- UPDATE ----------------------------------------------------------------

    public void Update()
    {
        if(IsDead) return;
        if(IsAttacking)
        {
            m_AttackTimer += Time.deltaTime;
            if(m_AttackTimer > m_AttackTrigger)
            {
                m_AttackTimer = 0;
                IsAttacking = false;
            }
        }

        if(m_IsInvulnerable)
        {
            m_InvulnerableTimer += Time.deltaTime;
            if(m_InvulnerableTimer > m_InvulnerableTrigger)
            {
                m_InvulnerableTimer = 0;
                m_IsInvulnerable = false;
            }
        }
    }

    //-- HEALTH DECREASE ----------------------------------------------------------------

    public void DecreaseHealthBy(int value)
    {
        if(m_IsInvulnerable) return;
        if (value > Data.Health) {
            Data.Health = 0;
        }
        else {
            Data.Health -= value;
        }

        if(Data.Health == 0)
        {
            OnDeath();
        }

        p_Puppet.state = Data.Health == 0 ? PuppetMaster.State.Dead : PuppetMaster.State.Alive;

        m_IsInvulnerable = true;

        if (OnTakeDamage != null) {
            OnTakeDamage();
        } else {
            Debug.LogWarning("OnTakeDamage not set!");
        }
    }

    public int CurrentHealth()
    {
        return Data.Health;
    }

    public void ResetHealth()
    {
        Data.Health = Data.MaxHealth;
        p_Puppet.state = PuppetMaster.State.Alive;
        OnTakeDamage(); // Update UI
        m_IsInvulnerable = false;
        IsAttacking = false;
    }

    public float HealthPercentage()
    {
        return (float)Data.Health / (float)Data.MaxHealth;
    }

    public int GetDamage() // eer not the best but will do for now
    {
        switch(m_LastAttackedLimb)
        {
            case Limb.HEAD:
                return 10;
            case Limb.TORSO:
                return 0;
            case Limb.RIGHT_ARM:
                return 5;
            case Limb.LEFT_ARM:
                return 8;
            case Limb.RIGHT_LEG:
                return 10;
            case Limb.LEFT_LEG:
                return 10;
            default:
                return 0;
        }
    }

    //-- COMMAND QUEUE -----------------------------------------------------------

    public void UpdateQueue()
    {
        QueueLogic();
    }

    public void ResetQueue()
    {
        m_QueueTrigger = 0;
        m_QueueTimer = 0;
        p_CommandQueue.Clear();
    }

    protected virtual void QueueLogic()
    {
        if(p_CommandQueue.Count <= 0) return;
        m_QueueTimer += Time.deltaTime;
        if(m_QueueTimer > m_QueueTrigger)
        {
            QueuedCommand command = p_CommandQueue.Dequeue();
            command.Execute();
            command = null;
            m_QueueTrigger = 1;// p_CommandQueue.Peek().ExecutionTime;
            m_QueueTimer = 0;
        }
    }

  //-- COMMANDS ----------------------------------------------------------------

    #region Commands

    public void MoveCommand(float x, float y)
    {
        if(p_UnitAnimator == null) return;

        Vector3 dir = new Vector3(y, 0, -x);
        float speed = dir.magnitude;

        p_UnitAnimator.SetFloat(AnimationID.MoveSpeed, speed);

        if(dir.x != 0 || dir.z != 0)
        {
            m_TargetRotAngle = Vector3.Angle(-Vector3.forward, dir);

            Vector3 cross = Vector3.Cross(Vector3.forward, dir);
            if(cross.y > 0)
            {
                m_TargetRotAngle = 360 - m_TargetRotAngle;
            }

            m_TargetRotAngle = Utility.ClampToCircle(m_TargetRotAngle + 90);
        }

        m_CurrentRotAngle = Mathf.SmoothDampAngle(m_CurrentRotAngle, m_TargetRotAngle, ref m_TargetRotVel, m_RotationSpeed);
        UnitObj.transform.rotation = Quaternion.Euler(0, m_CurrentRotAngle, 0);


        //m_TargetRotAngle = Vector3.Angle(UnitObj.transform.forward, dir) - 90;

        //Vector3 cross = Vector3.Cross(UnitObj.transform.forward, dir);
        //if(cross.z > 0)
        //{
        //    m_TargetRotAngle = 360 - m_TargetRotAngle;
        //}

        //m_CurrentRotAngle = Mathf.SmoothDamp(m_CurrentRotAngle, m_TargetRotAngle, ref m_TargetRotVel, m_RotationSpeed);

        //Debugger.Log("Target Angle: " + Mathf.Round(m_TargetRotAngle), DebuggerTags.DBTag.Testing);
        ////Debugger.Log("Current Angle: " + m_CurrentRotAngle, DebuggerTags.DBTag.Testing);

        //UnitObj.transform.Rotate(0, m_TargetRotAngle, 0);
    }

    public void QueueAttackCommand(Limb limb)
    {
        if(HasFinishedTurn) return;
        Limb l = limb;
        QueuedCommand com = new QueuedCommand(()=> AttackCommand(l), 1); // To-Do Read attack data
        p_CommandQueue.Enqueue(com);
    }

    public void AttackCommand(Limb limb)
    {
        string animID = AnimationID.GetAttackLimb(limb);
        if(animID == null) return;

        m_LastAttackedLimb = limb;
        p_UnitAnimator.SetTrigger(animID);
        m_AttackTrigger = 1.2f; //UnitAnimator.GetCurrentAnimatorClipInfo(0).Length;
        IsAttacking = true;
        
        AudioManager.Instance.Play3DAudio(Resources.Load<AudioClip>("Audio/Woosh"), UnitObj.transform.position, 30, 40);
    }


    public void QueueBlockCommand()
    {
        if(HasFinishedTurn) return;
        QueuedCommand com = new QueuedCommand(() => BlockCommand(), 1);
        p_CommandQueue.Enqueue(com);
    }

    public void BlockCommand()
    {
        string animID = AnimationID.GetBlock();
        if(animID == null) return;

        p_UnitAnimator.SetTrigger(animID);
    }


    public void QueueIdleCommand()
    {
        if(HasFinishedTurn) return;
        QueuedCommand com = new QueuedCommand(() => IdleCommand(), 1);
        p_CommandQueue.Enqueue(com);
    }

    public void IdleCommand()
    {
        string animID = AnimationID.GetIdle();
        if(animID == null) return;

        p_UnitAnimator.SetTrigger(animID);
    }


    public void QueueTauntCommand()
    {
        if(HasFinishedTurn) return;
        QueuedCommand com = new QueuedCommand(() => TauntCommand(), 1);
        p_CommandQueue.Enqueue(com);
    }

    public void TauntCommand()
    {
        string animID = AnimationID.GetTaunt();
        if(animID == null) return;

        p_UnitAnimator.SetTrigger(animID);
    }

    #endregion

    //-- IK FUNCTIONS ------------------------------------------------------------

    public void UpdateIK()
    {

    }


    //-- HELPER FUNCTIONS --------------------------------------------------------


    //----------------------------------------------------------------------------

    protected class QueuedCommand
    {
        private Action m_Command;
        public float ExecutionTime { get; private set; }

        public QueuedCommand(Action command, float executionTime)
        {
            m_Command = command;
            ExecutionTime = executionTime;
        }

        public void Execute()
        {
            m_Command();
        }
    }
}
