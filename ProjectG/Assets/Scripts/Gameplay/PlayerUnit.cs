using UnityEngine;
using System.Collections;

public class PlayerUnit : Unit
{
    //-----------------------------------------------------------------



    //-----------------------------------------------------------------

    public PlayerUnit(string prefabName, Vector3 spawnPos) : base(prefabName)
    {
        Debug.Log("Creating new " + prefabName + "unit");
        UnitParentObj.transform.position = spawnPos;
    }

    //-----------------------------------------------------------------


    //-----------------------------------------------------------------
}
