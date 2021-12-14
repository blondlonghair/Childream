using System;
using UnityEditor;

#if  UNITY_EDITOR
using UnityEditor.SceneManagement;

// 에디터에서 씬을 빠르고 쉽게 열어서 작업할수 있도록 도와줍니다.

public class EditorSceneOpen
{
    [MenuItem("Scenes/1.IngameScene")]
    public static void OpenScene_Title()
    {
        OpenScene("Assets/1_Scenes/IntroScene.unity");
    }

    [MenuItem("Scenes/2.LobbyScene")]
    public static void OpenScene_Lobby()
    {
        OpenScene("Assets/1_Scenes/LobbyScene.unity");
    }
    
    [MenuItem("Scenes/3.IngameScene")]
    public static void OpenScene_Ingame()
    {
        OpenScene("Assets/1_Scenes/IngameScene.unity");
    }
    
    [MenuItem("Scenes/4.TutorialScene")]
    public static void OpenScene_Tutorial()
    {
        OpenScene("Assets/1_Scenes/TutorialScene.unity");
    }
    
    [MenuItem("Scenes/5.Test")]
    public static void OpenScene_Test()
    {
        OpenScene("Assets/1_Scenes/Test.unity");
    }
    
    public static void OpenScene(string scenepath)
    {
        if(EditorSceneManager.GetActiveScene().isDirty == true)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }
        EditorSceneManager.OpenScene(scenepath);
    }
}
#endif
