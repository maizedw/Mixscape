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
        base.OnInspectorGUI();

        if (GUILayout.Button("Set to Next Skybox"))
        {
            TargetLevelManager.NextSkybox();
        }
    }
}
