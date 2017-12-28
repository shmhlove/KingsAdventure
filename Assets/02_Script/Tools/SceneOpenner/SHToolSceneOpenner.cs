#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

using System;
using System.Collections;

public class SHToolSceneOpenner : Editor 
{
    static void SaveScene(string strPath)
    {
        EditorSceneManager.SaveScene(SceneManager.GetSceneByPath(strPath));
    }

    static void OpenScene(string strPath)
    {
        EditorSceneManager.OpenScene(strPath);
    }

    static bool EqualsScene(string strPath)
    {
        if (false == SceneManager.GetActiveScene().path.Equals(strPath))
            return false;
        
        EditorUtility.DisplayDialog("Warning", "Same Scene..", "Ok");
        return true;
    }

    static bool CheckMovePossibleState()
    {
        if ((true == EditorApplication.isPlaying)   ||
            (true == EditorApplication.isCompiling) ||
            (true == EditorApplication.isPaused)    ||
            (true == EditorApplication.isUpdating))
        {
            EditorUtility.DisplayDialog("Error", "Unity is Busy..\n\n(Playing || Compiling || Paused || Updating..)", "Ok");
            return true;
        }

        return false;
    }

    static void SaveCurrentScene()
    {
        if (true == EditorUtility.DisplayDialog("Save Scene", "Are You Save Current Scene?", "Ok", "Cancel"))
        {
            SaveScene(SceneManager.GetActiveScene().path);
        }
    }

    public static void LoadScene(string strScenePath)
    {
        if (true == CheckMovePossibleState())
            return;

        if (true == EqualsScene(strScenePath))
            return;

        SaveCurrentScene();

        OpenScene(strScenePath);
    }

    [MenuItem("SHSceneOpener/01_Intro")]
    static void LoadScene_Intro()
    {
        SHToolSceneOpenner.LoadScene("Assets/01_Scene/Intro.unity");
    }
    [MenuItem("SHSceneOpener/02_Patch")]
    static void LoadScene_Patch()
    {
        SHToolSceneOpenner.LoadScene("Assets/01_Scene/Patch.unity");
    }
    [MenuItem("SHSceneOpener/03_Login")]
    static void LoadScene_Login()
    {
        SHToolSceneOpenner.LoadScene("Assets/01_Scene/Login.unity");
    }
    [MenuItem("SHSceneOpener/Loading")]
    static void LoadScene_Loading()
    {
        SHToolSceneOpenner.LoadScene("Assets/01_Scene/Loading.unity");
    }
}
#endif