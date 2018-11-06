using UnityEngine;
using System.Collections;

public class Command
{
    public delegate void ExecuteCommand( Unit unit );
    public ExecuteCommand Execute;

    public Command()
    {
        Execute = (u)=> Debug.Log("Null Command");
    }
}
