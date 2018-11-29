using UnityEngine;
using System.Collections;

public class PlayerUnit : Unit
{
    //-----------------------------------------------------------------



    //-----------------------------------------------------------------

    public PlayerUnit(string prefabName, string name, int unitId, Vector3 spawnPos) : base(prefabName, name, unitId)
    {
        UnitParentObj.transform.position = spawnPos;
    }

    //-----------------------------------------------------------------


    //-----------------------------------------------------------------
}
