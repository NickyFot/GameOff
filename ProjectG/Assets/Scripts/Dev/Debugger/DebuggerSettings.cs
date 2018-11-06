using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Debugger Settings", menuName = "Debugger/Settings", order = 1)]
public class DebuggerSettings : ScriptableObject
{
    public Color ValueColour = Color.white;

    public List<DebuggerTag> Tags = new List<DebuggerTag>();
}
