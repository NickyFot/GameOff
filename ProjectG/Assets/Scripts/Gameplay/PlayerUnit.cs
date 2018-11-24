using UnityEngine;
using System.Collections;

public class PlayerUnit : Unit
{
    //-----------------------------------------------------------------



    //-----------------------------------------------------------------

    public PlayerUnit(string prefabName, string name, Vector3 spawnPos) : base(prefabName, name)
    {
        Debug.Log("Creating new " + prefabName + "unit");
        UnitParentObj.transform.position = spawnPos;
    }

    //-----------------------------------------------------------------


    //-----------------------------------------------------------------
}
