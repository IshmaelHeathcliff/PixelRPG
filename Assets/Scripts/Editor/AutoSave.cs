using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class AutoSave
{
    static AutoSave()
    {
        EditorApplication.playModeStateChanged += SaveOnPlay;
    }

    private static void SaveOnPlay(PlayModeStateChange state)
    {
        if(state==PlayModeStateChange.ExitingEditMode)
        {
            Debug.Log("Auto-saving ...");

            EditorSceneManager.SaveOpenScenes();

            AssetDatabase.SaveAssets();
        }
    }
}

