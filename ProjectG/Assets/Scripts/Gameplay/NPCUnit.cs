using UnityEngine;
using System.Collections;

public class NPCUnit : Unit
{
    //-----------------------------------------------------------------



    //-----------------------------------------------------------------

    public NPCUnit(string prefabName, string name, int unitId, Vector3 spawnPos) : base(prefabName, name, unitId)
    {
        UnitParentObj.transform.position = spawnPos;
    }

    //-----------------------------------------------------------------


    //-----------------------------------------------------------------
}
