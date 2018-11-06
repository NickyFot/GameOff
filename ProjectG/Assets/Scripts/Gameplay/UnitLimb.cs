using UnityEngine;
using RootMotion.FinalIK;
using System.Collections;

public class UnitLimb
{
    //----------------------------------------------
    // LIMB TYPE
    //----------------------------------------------

    public enum LimbType
    {
        HEAD,
        TORSO,
        RIGHT_ARM,
        LEFT_ARM,
        RIGHT_LEG,
        LEFT_LEG
    }
    public LimbType LimbPart;

    //----------------------------------------------
    // MAIN LIMB STATS
    //----------------------------------------------



    //----------------------------------------------
    // GAMEPLAY VARS
    //----------------------------------------------   

    public const bool IsControllable = false;

    private CharacterJoint m_Joint;

    private float m_MaxBreakForce;
    private float m_BreakForce;
    public float BreakingForce { get { return m_BreakForce; } }

    //----------------------------------------------

    public UnitLimb(LimbType limb, float maxBreakForce)
    {
        LimbPart = limb;

        m_MaxBreakForce = maxBreakForce;
        m_BreakForce = maxBreakForce;
    }

    //----------------------------------------------

    public void Update()
    {

    }

    //----------------------------------------------
}
