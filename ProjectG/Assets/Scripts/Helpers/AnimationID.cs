﻿public static class AnimationID
{
    // UNIT ANIMATIONS
    public const string MoveSpeed       = "MoveSpeed";

    public const string AttackRightArm  = "AttackRightArm";
    public const string AttackLeftArm   = "AttackLeftArm";
    public const string AttackRightLeg  = "AttackRightLeg";
    public const string AttackLeftLeg   = "AttackLeftLeg";

    public static string GetAttackLimb(UnitLimb.LimbType limb)
    {
        switch(limb)
        {
            case UnitLimb.LimbType.RIGHT_ARM:
                return AttackRightArm;
            case UnitLimb.LimbType.LEFT_ARM:
                return AttackLeftArm;
            case UnitLimb.LimbType.RIGHT_LEG:
                return AttackRightLeg;
            case UnitLimb.LimbType.LEFT_LEG:
                return AttackLeftLeg;
            default:
                return null;
        }
    }
}