using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LevelManager : MonoBehaviour
{
    public Material[] SkyboxMaterials;
    private int CurrentSkybox;
    
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < SkyboxMaterials.Length; ++i)
        {
            if (RenderSettings.skybox == SkyboxMaterials[i])
            {
                CurrentSkybox = i;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void NextSkybox()
    {
        if (++CurrentSkybox >= SkyboxMaterials.Length)
        {
            CurrentSkybox = 0;
        }
        RenderSettings.skybox = SkyboxMaterials[CurrentSkybox];

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        }
#endif
    }
}
