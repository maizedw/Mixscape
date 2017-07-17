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
        EditorGUILayout.LabelField("Level Editing", EditorStyles.boldLabel);

        if (GUILayout.Button("Select Terrain"))
        {
            Terrain terrain = TargetLevelManager.transform.GetComponentInChildren<Terrain>();
            if (terrain == null) terrain = GameObject.FindObjectOfType<Terrain>();
            Select(terrain);
        }

        if (GUILayout.Button("Set to Next Skybox"))
        {
            NextSkybox();
        }

        if (_levelInfo != null)
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Prefab Select", EditorStyles.boldLabel);

            DoSelectRandomPrefabButton("Blocking Rocks", _levelInfo.LevelBlockingObjects);
            DoSelectRandomPrefabButton("Trees", _levelInfo.TreeObjects);
            DoSelectRandomPrefabButton("Bushes", _levelInfo.BushObjects);
            DoSelectRandomPrefabButton("Mushrooms", _levelInfo.MushroomObjects);
            DoSelectRandomPrefabButton("Rocks", _levelInfo.RockObjects);
            DoSelectRandomPrefabButton("Human Made", _levelInfo.HumanMadeObjects);
        }
    }

    public void DoSelectRandomPrefabButton(string label, GameObject[] objectArray)
    {
        if (GUILayout.Button(label))
        {
            Select(PullRandom(objectArray));
        }
    }

    public static void Select(Component component)
    {
        if (component != null)
        {
            Selection.activeGameObject = component.gameObject;
        }
    }

    public static void Select(GameObject obj)
    {
        if (obj != null)
        {
            Selection.activeGameObject = obj;
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

    public GameObject PullRandom(GameObject[] objectArray)
    {
        if (objectArray != null && objectArray.Length > 0)
        {
            return objectArray[Random.Range(0, objectArray.Length)];
        }
        return null;
    }
}
