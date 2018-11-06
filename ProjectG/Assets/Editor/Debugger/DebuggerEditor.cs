using UnityEditor;
using UnityEngine;
using System.IO;

[CustomEditor(typeof(DebuggerSettings))]
public class DebuggerEditor : Editor
{
    //-----------------------------------------------------------------

    private DebuggerSettings settings;

    private DebuggerTag m_SelectedTag;

    //-----------------------------------------------------------------

    public override void OnInspectorGUI()
    {
        settings = (DebuggerSettings) target;
        DrawValueColor();
        DrawTagsMenu();
        DrawSetActive();
        if(settings == null) return;
        EditorUtility.SetDirty(settings);
    }

    //-----------------------------------------------------------------

    private void DrawValueColor()
    {
        EditorGUILayout.LabelField("Value Colour", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Change the colour of the numerical values", EditorStyles.miniLabel);

        settings.ValueColour = EditorGUILayout.ColorField("Colour", settings.ValueColour);
    }

    private void DrawTagsMenu()
    {
        EditorGUILayout.LabelField("Debugger Tags", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Create your own custom tags", EditorStyles.miniLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        for(int i = 0; i < settings.Tags.Count; i++)
        {
            GUIStyle style = EditorStyles.label;
            string name = string.IsNullOrEmpty(settings.Tags[i].TagName) ? "Tag " + i : settings.Tags[i].TagName;
            if(m_SelectedTag != null)
            {
                if(settings.Tags[i] == m_SelectedTag)
                {
                    style = EditorStyles.label;
                }
            }

            EditorGUILayout.BeginHorizontal("ObjectFieldThumb", GUILayout.Height(21));

            settings.Tags[i].TagName = EditorGUILayout.TextField(settings.Tags[i].TagName, GUILayout.ExpandWidth(true));
            settings.Tags[i].TagColor = EditorGUILayout.ColorField(settings.Tags[i].TagColor, GUILayout.ExpandWidth(false));
           // settings.Tags[i].TagPriority = EditorGUILayout.IntField("Priority", settings.Tags[i].TagPriority, GUILayout.ExpandWidth(false));
            if(GUILayout.Button("", "OL Minus"))
            {
                if(EditorUtility.DisplayDialog("WAIT!", "Are you sure you want to delete the selected tag? All data will be lost.", "Yes", "No"))
                {
                    settings.Tags.RemoveAt(i);
                    m_SelectedTag = null;
                }
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);
        }

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Add", EditorStyles.toolbarButton))
        {
            settings.Tags.Add(new DebuggerTag());
        }

        //if(GUILayout.Button("Remove", EditorStyles.toolbarButton))
        //{
        //    if(m_SelectedTag != null)
        //    {
        //        if(EditorUtility.DisplayDialog("WAIT!", "Are you sure you want to delete the selected tag? All data will be lost.", "Yes", "No"))
        //        {
        //            settings.Tags.Remove(m_SelectedTag);
        //            m_SelectedTag = null;
        //        }
        //    }
        //}

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        if(GUI.changed)
        {
            //AssetDatabase.SaveAssets();
        }
    }

    private void DrawSetActive()
    {
        GUILayout.Space(10);
        if(GUILayout.Button("Set Active Profile", GUILayout.Height(45)))
        {
            if(EditorUtility.DisplayDialog("WAIT!", "Are you sure you want to make this the active debugger profile?", "Yes", "No"))
            {
                SetActiveProfile();
            }
        }
    }

    //-----------------------------------------------------------------

    private void SetActiveProfile()
    {
        string path = Application.dataPath + "/Scripts/Dev/Debugger/DebuggerTags.cs";
        TextWriter tw = new StreamWriter(path);

        tw.Write("public class DebuggerTags\n" +
            "{\n" +
            "\tpublic enum DBTag\n" +
            "\t{\n");

        for(int i = 0; i < settings.Tags.Count; i++)
        {
            string coma = i == settings.Tags.Count - 1 ? "" : ",";
            tw.WriteLine("\t\t" + settings.Tags[i].TagName + coma);
        }

        tw.Write("\t}\n" +
            "}\n");

        tw.Close();
    }

    //-----------------------------------------------------------------
}