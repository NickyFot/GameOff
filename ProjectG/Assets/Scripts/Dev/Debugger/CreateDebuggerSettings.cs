using UnityEditor;
using UnityEngine;

public class CreateDebuggerSettings
{
    [MenuItem("Assets/Create/Debugger/DebuggerProfile")]
    public static void CreateSettings()
    {
        DebuggerSettings asset = ScriptableObject.CreateInstance<DebuggerSettings>();

        AssetDatabase.CreateAsset(asset, "Assets/Resources/DebuggerSettings.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
