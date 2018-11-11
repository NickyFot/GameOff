using UnityEngine;
using System.Collections;

public class InputCommand
{
    public delegate void ExecuteCommand( Unit unit );
    public ExecuteCommand Execute;

    public InputCommand()
    {
        Execute = (u)=> Debug.Log("Null Command");
    }
}
