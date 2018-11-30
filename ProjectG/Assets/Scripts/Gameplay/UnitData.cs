using UnityEngine;
using System.Collections;

public class UnitData
{
    private static string[] m_MatPaths = new string[]{ "Materials/SharkDude_Green", "Materials/SharkDude_Red", "Materials/SharkDude_Yellow", "Materials/SharkDude_Pink" };

    private const string m_SmokePrefabPath = "Prefabs/Particles/Smoke_Hit";
    public static GameObject SmokePrefab;

    // HP - Stamina...
    public string Name;
    public int Health = 100;
    public readonly int MaxHealth = 100;
    public int MaxQueueInput = 3;

    public UnitData(string name)
    {
        this.Name = name;
        SmokePrefab = Resources.Load<GameObject>(m_SmokePrefabPath);
    }

    public static Material GetMaterial(int index)
    {
        if(index > m_MatPaths.Length) return null;
        return Resources.Load<Material>(m_MatPaths[index]);
    }

    public static RenderTexture GetFaceCamTexture(int id)
    {
        return Resources.Load<RenderTexture>("UITextures/PlayerTex" + ( id + 1 ).ToString());
    }

}
