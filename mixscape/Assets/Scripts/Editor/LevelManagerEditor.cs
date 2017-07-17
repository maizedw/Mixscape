using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelManager), true)]
public class LevelManagerEditor : Editor
{
    public LevelManager TargetLevelManager { get { return target as LevelManager; } }

    private LevelInfo _levelInfo = null;
    private int _currentSkybox;
    
    void OnEnable()
    {
        LoadLevelSettings();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Separator();

        if (GUILayout.Button("Select Terrain"))
        {
            Terrain terrain = TargetLevelManager.transform.GetComponentInChildren<Terrain>();
            if (terrain == null) terrain = GameObject.FindObjectOfType<Terrain>();

            if (terrain != null)
            {
                Selection.activeGameObject = terrain.gameObject;
            }
        }

        if (GUILayout.Button("Set to Next Skybox"))
        {
            NextSkybox();
        }

    }

    public void LoadLevelSettings()
    {
        _levelInfo = Resources.Load<LevelInfo>("LevelSettings");
        if (_levelInfo == null)
        {
            _levelInfo = new LevelInfo();
            AssetDatabase.CreateAsset(_levelInfo, "Assets/Resources/LevelSettings.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        InitSkyboxSettings();
    }

    public void InitSkyboxSettings()
    {
        if (_levelInfo == null) return;

        for (int i = 0; i < _levelInfo.SkyboxMaterials.Length; ++i)
        {
            if (RenderSettings.skybox == _levelInfo.SkyboxMaterials[i])
            {
                _currentSkybox = i;
                break;
            }
        }
    }

    public void NextSkybox()
    {
        if (_levelInfo == null) return;

        if (++_currentSkybox >= _levelInfo.SkyboxMaterials.Length)
        {
            _currentSkybox = 0;
        }

        if (_currentSkybox < _levelInfo.SkyboxMaterials.Length)
        {
            RenderSettings.skybox = _levelInfo.SkyboxMaterials[_currentSkybox];

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
            }
        }
#endif
    }
}
