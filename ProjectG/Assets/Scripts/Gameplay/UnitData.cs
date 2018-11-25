using UnityEngine;
using System.Collections;

public class UnitData
{
    // HP - Stamina...
    public string Name;
    public int Health = 100;
    public int MaxHealth = 100;
    public int MaxQueueInput = 3;

    public UnitData(string name) {
        this.Name = name;
    }
}
