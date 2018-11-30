using UnityEditor;
using UnityEditor.SceneManagement;

public class ScenesShortcuts
{
    private static void OpenScene(string path)
    {
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(path);
        }
    }

    //MAIN
    [MenuItem("Scenes/Main", priority = 50)]
    private static void Main_Level()
    {
        OpenScene("Assets/Scenes/Main.unity");
    }

    [MenuItem("Scenes/Splash", priority = 51)]
    private static void Splash()
    {
        OpenScene("Assets/Scenes/Splash.unity");
    }

    [MenuItem("Scenes/Main Menu", priority = 51)]
    private static void Main_Menu()
    {
        OpenScene("Assets/Scenes/MainMenu.unity");
    }

    [MenuItem("Scenes/Dev/BrawlDemo", priority = 52)]
    private static void Brawl_Demo()
    {
        OpenScene("Assets/Scenes/Dev/BrawlDemo.unity");
    }

    [MenuItem("Scenes/Dev/GDsTestBed", priority = 53)]
    private static void Gd_Demo()
    {
        OpenScene("Assets/Scenes/Dev/GDsTestBed.unity");
    }
}
