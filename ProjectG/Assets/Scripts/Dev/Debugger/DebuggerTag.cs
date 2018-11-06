using UnityEngine;

[System.Serializable]
public class DebuggerTag
{
    public string TagName;
    public Color TagColor;
    public int TagPriority;

    public DebuggerTag()
    {
        TagName = "New Tag";
        TagColor = Color.white;
        TagPriority = -1;
    }
}
