using UnityEngine;
using System.Collections;

public class UnitData
{
    private static string[] m_MatPaths = new string[]{ "Materials/SharkDude_Green", "Materials/SharkDude_Red", "Materials/SharkDude_Yellow", "Materials/SharkDude_Pink" };

    public static Material GetMaterial(int index)
    {
        if(index > m_MatPaths.Length) return null;
        return Resources.Load<Material>(m_MatPaths[index]);
    }

    // HP - Stamina...
    public string Name;
    public int Health = 100;
    public readonly int MaxHealth = 100;
    public int MaxQueueInput = 3;

    public UnitData(string name) {
        this.Name = name;
    }
}
