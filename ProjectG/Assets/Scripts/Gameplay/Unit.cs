using UnityEngine;
using RootMotion.FinalIK;
using System.Collections;
using RootMotion.Dynamics;

using Limb = UnitLimb.LimbType;

public abstract class Unit
{
    //-- VARIABLES --------------------------------------------------------------
    // -- Data
    public UnitData Data;

    // -- Object
    public GameObject UnitParentObj;
    public GameObject UnitObj;

    // -- Animators
    protected Animator UnitAnimator;
    protected FullBodyBipedIK BodyIK;


    // -- Gameplay Vars
    // To-DO: Expose a scriptable object or something (coonet to Unit Data)
    private float m_RotationSpeed = 0.2f;

    // -- Movement Vars
    private float m_CurrentRotAngle;
    private float m_TargetRotAngle;
    private float m_TargetRotVel;

    // -- IK Vars

    //Static Limbs
    protected LimbSlot p_HeadSlot;
    protected LimbSlot p_TorsoSlot;

    // Controllable Limbs
    protected LimbSlot p_RightArmSlot; 
    protected LimbSlot p_LeftArmSlot; 
    protected LimbSlot p_RightLegSlot; 
    protected LimbSlot p_LeftLegSlot;

    //-- CONSTRUCTOR -------------------------------------------------------------

    public Unit(string prefabName)
    {
        UnitParentObj = Object.Instantiate(Resources.Load<GameObject>(prefabName));
        UnitAnimator = UnitParentObj.GetComponentInChildren<Animator>();
        UnitObj = UnitAnimator.gameObject;
        BodyIK = UnitObj.GetComponent<FullBodyBipedIK>();
    }

    //-- COMMANDS ----------------------------------------------------------------

    #region Commands

    public void MoveCommand(float x, float y)
    {
        if(UnitAnimator == null) return;

        Vector3 dir = new Vector3(y, 0, -x);
        float speed = dir.magnitude;

        UnitAnimator.SetFloat(AnimationID.MoveSpeed, speed);

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

    public void AttackCommand(Limb limb)
    {
        //LimbSlot attackingLimbSlot = GetLimbSlot(limb);
        //if(attackingLimbSlot == null) return;
        //if(!attackingLimbSlot.IsControllable || !attackingLimbSlot.IsActive) return;

        string animID = AnimationID.GetAttackLimb(limb);
        if(animID == null) return;

        UnitAnimator.SetTrigger(animID);
    }

    public void ControlLimbCommand(Limb limbSlot)
    {
        LimbSlot attackingLimbSlot = GetLimbSlot(limbSlot);
        if(attackingLimbSlot == null) return;
        if(!attackingLimbSlot.IsControllable || !attackingLimbSlot.IsActive) return;

        ControllableUnitLimb limb = attackingLimbSlot.AttachedLimb as ControllableUnitLimb;

    }

    #endregion

    //-- IK FUNCTIONS ------------------------------------------------------------

    public void UpdateIK()
    {

    }

    //********************** DEBUG ****************************
    public MuscleRemoveMode removeMuscleMode = MuscleRemoveMode.Explode;
    public float unpin = 10f;
    public float force = 10f;
    public ParticleSystem particles;

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast to find a ragdoll collider
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask("CharacterController")))
            {
                MuscleCollisionBroadcaster broadcaster = hit.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

                // If is a muscle...
                if(broadcaster != null)
                {
                    broadcaster.Hit(unpin, ray.direction * force, hit.point);

                    // Remove the muscle and its children
                    broadcaster.puppetMaster.RemoveMuscleRecursive(broadcaster.puppetMaster.muscles[broadcaster.muscleIndex].joint, true, true, removeMuscleMode);
                }
                else
                {
                    // Not a muscle (any more)
                    var joint = hit.collider.attachedRigidbody.GetComponent<ConfigurableJoint>();
                    if(joint != null) GameObject.Destroy(joint);

                    // Add force
                    hit.collider.attachedRigidbody.AddForceAtPosition(ray.direction * force, hit.point);
                }

                // Particle FX
                //particles.transform.position = hit.point;
                //particles.transform.rotation = Quaternion.LookRotation(-ray.direction);
                //particles.Emit(5);
            }
        }
    }
    //********************** DEBUG ****************************

    //-- HELPER FUNCTIONS --------------------------------------------------------

    public LimbSlot GetLimbSlot(Limb limb)
    {
        switch(limb)
        {
            case Limb.HEAD:
                return p_HeadSlot;
            case Limb.TORSO:
                return p_TorsoSlot;
            case Limb.RIGHT_ARM:
                return p_RightArmSlot;
            case Limb.LEFT_ARM:
                return p_LeftArmSlot;
            case Limb.RIGHT_LEG:
                return p_RightLegSlot;
            case Limb.LEFT_LEG:
                return p_LeftLegSlot;
            default:
                return null;
        }
    }

    //----------------------------------------------------------------------------
}
