using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbSlot
{
    //----------------------------------------------

    public UnitLimb AttachedLimb;

    public bool IsActive;
    public bool IsControllable { get; private set; }

    //----------------------------------------------

    public LimbSlot(bool isControllable)
    {
        IsControllable = isControllable;
    }

    //----------------------------------------------

    public void UpdateLimb(float IKTargetWeight)
    {
        if(AttachedLimb == null) return;
        if(!IsActive) return;
    }

    //----------------------------------------------

    private void InterpolateIKTargetToLimbSlot(float IKTargetWeight)
    {

    }

    //----------------------------------------------
}
