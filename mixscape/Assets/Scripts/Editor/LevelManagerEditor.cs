using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager), true)]
public class LevelManagerEditor : Editor
{
    public LevelManager TargetLevelManager { get { return target as LevelManager; } }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Change Skybox"))
        {
            TargetLevelManager.NextSkybox();
        }

        base.OnInspectorGUI();
    }
}
