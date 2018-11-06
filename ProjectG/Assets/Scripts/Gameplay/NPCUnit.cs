using UnityEngine;
using System.Collections;

public class NPCUnit : Unit
{
    //-----------------------------------------------------------------



    //-----------------------------------------------------------------

    public NPCUnit(string prefabName, Vector3 spawnPos) : base(prefabName)
    {
        UnitParentObj.transform.position = spawnPos;
    }

    //-----------------------------------------------------------------


    //-----------------------------------------------------------------
}
