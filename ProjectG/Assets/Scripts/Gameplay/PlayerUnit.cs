using UnityEngine;
using System.Collections;

public class PlayerUnit : Unit
{
    //-----------------------------------------------------------------



    //-----------------------------------------------------------------

    public PlayerUnit(string prefabName, Vector3 spawnPos) : base(prefabName)
    {
        Debugger.Log("Creating new " + prefabName + "unit", DebuggerTags.DBTag.Unit);
        UnitParentObj.transform.position = spawnPos;
    }

    //-----------------------------------------------------------------


    //-----------------------------------------------------------------
}
