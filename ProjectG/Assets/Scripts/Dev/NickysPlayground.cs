using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NickysPlayground : MonoBehaviour
{
	void Start ()
    {
        WeaponStats nickyIsGreat = new WeaponStats(10, 20);
        Debugger.Log(nickyIsGreat.ToString(), DebuggerTags.DBTag.Testing);
        WeaponStats stathIsStupid = new WeaponStats(5, 5);
        nickyIsGreat -= stathIsStupid;
        Debugger.Log(nickyIsGreat.ToString(), DebuggerTags.DBTag.Testing);
	}

    void Update ()
    {
		
	}
}
