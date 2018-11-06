using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using Tags = DebuggerTags.DBTag;

public static class Debugger
{
    //-----------------------------------------------------------------

    private static DebuggerSettings m_Settings;
    private static DebuggerSettings Settings
    {
        get
        {
            if(m_Settings == null)
            {
                m_Settings = Resources.Load<DebuggerSettings>("Debugger/DefaultSettings");
            }
            return m_Settings;
        }
    }

    private static Regex m_NumAlpha = new Regex("(?<Alpha>[a-zA-Z]*)(?<Numeric>[0-9]*)");

    //-----------------------------------------------------------------

    public static void Log(string content)
    {
        //bool hasDigit = content.Any(c => char.IsDigit(c));
        //if(hasDigit)
        //{
        //    Color col = Settings.ValueColour;
        //    string hex = ColorUtility.ToHtmlStringRGB(col);

        //    Match match = m_NumAlpha.Match(content);

        //    string text = match.Groups["Alpha"].Value;
        //    string num = match.Groups["Numeric"].Value;

        //    num = "<color=#" + hex + ">" + num + "</color>"; // To-Do: Make more efficient!

        //    Debug.Log(text);
        //    Debug.Log(num);
        //}

        Debug.Log(content);
    }

    public static void Log(string content, Tags tag)
    {
        DebuggerTag tagData = EnumToTag(tag);
        string hex = ColorUtility.ToHtmlStringRGB(tagData.TagColor);
        string tagTxt = "<b><color=#" + hex + ">[" + tagData.TagName + "]</color></b> ";
        Log(tagTxt + content);
    }

    //-----------------------------------------------------------------

    private static DebuggerTag EnumToTag(Tags enumTag)
    {
        for(int i = 0; i < Settings.Tags.Count; i++)
        {
            if(enumTag.ToString() == Settings.Tags[i].TagName)
            {
                return Settings.Tags[i];
            }
        }
        return null;
    }

    //-----------------------------------------------------------------
}
