using UnityEngine;
using System.Collections;

public class NPCUnit : Unit
{
    //-----------------------------------------------------------------



    //-----------------------------------------------------------------

    public NPCUnit(string prefabName, string name, Vector3 spawnPos) : base(prefabName, name)
    {
        UnitParentObj.transform.position = spawnPos;
    }

    //-----------------------------------------------------------------


    //-----------------------------------------------------------------
}
