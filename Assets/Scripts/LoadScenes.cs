using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using System.IO;

/// <summary>
/// Controller for Loading CityEngine Unity Scenes
/// </summary>
public class LoadScenes : MonoBehaviour
{
    [Header("CityEngine Unity Scenes")]
    /// <summary>
    /// Existing Conditions CityEngine Model
    /// </summary>
    public SceneField ExistingConditionsModel;

    /// <summary>
    /// Demolition CityEngine Model
    /// </summary>
    //public Scene DemolitionModel;
    public SceneField DemolitionModel;

    /// <summary>
    /// Demolition CityEngine Model
    /// </summary>
    //public Scene Scenario1Model;
    public SceneField Scenario1Model;

    /// <summary>
    /// Demolition CityEngine Model
    /// </summary>
    //public Scene Scenario2Model;
    public SceneField Scenario2Model;

    /// <summary>
    /// Demolition CityEngine Model
    /// </summary>
    public SceneField Scenario3Model;

    /// <summary>
    /// Disable Directional Lights
    /// </summary>
    private bool DisableDirectionalLights = false;


    private int xPos = 0;
    private int yPos = 0;
    private int zPos = 0;

    public void AddSceneToBuild(SceneField inputScene)
    {
        {
            string[] fullFilePath0 = Directory.GetFiles(Application.dataPath, inputScene.SceneName + ".unity", SearchOption.AllDirectories);
            string pathOfSceneToAdd = "Assets" + fullFilePath0[0].Substring(Application.dataPath.Length).Replace("\\", "/");

            //Loop through and see if the scene already exist in the build settings
            int indexOfSceneIfExist = -1;
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].path == pathOfSceneToAdd)
                {
                    indexOfSceneIfExist = i;
                    break;
                }
            }

            EditorBuildSettingsScene[] newScenes;

            if (indexOfSceneIfExist == -1)
            {
                newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];

                //Seems inefficent to add scene to build settings after creating each scene (rather than doing it all at once
                //after they are all created, however, it's necessary to avoid memory issues.
                int i = 0;
                for (; i < EditorBuildSettings.scenes.Length; i++)
                    newScenes[i] = EditorBuildSettings.scenes[i];

                newScenes[i] = new EditorBuildSettingsScene(pathOfSceneToAdd, true);
            }
            else
            {
                newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length];

                int i = 0, j = 0;
                for (; i < EditorBuildSettings.scenes.Length; i++)
                {
                    //skip over the scene that is a duplicate
                    //this will effectively delete it from the build settings
                    if (i != indexOfSceneIfExist)
                        newScenes[j++] = EditorBuildSettings.scenes[i];
                }
                newScenes[j] = new EditorBuildSettingsScene(pathOfSceneToAdd, true);
            }

            EditorBuildSettings.scenes = newScenes;
        }
    }

    private bool ScenariosAddedtoBuild = false;

    void Awake()
    {
        /*
    #if UNITY_EDITOR
            // TODO Geof7015: Cleanup code below and parse scenes into "AddScene2BuildSettings()"
            // Adding Scenes to Build Settings
            AddSceneToBuild(ExistingConditionsModel);
            AddSceneToBuild(DemolitionModel);
            AddSceneToBuild(Scenario1Model);
            AddSceneToBuild(Scenario2Model);
            AddSceneToBuild(Scenario3Model);
    #endif
        */
        ScenariosAddedtoBuild = true;
    }


    private void Update()
    { if (ScenariosAddedtoBuild)
        {
            if (Input.GetKeyDown("a"))
            {
                ComputeScene(DemolitionModel);
            }

            else if (Input.GetKeyDown("s"))
            {
                ComputeScene(Scenario1Model);
            }
            else if (Input.GetKeyDown("d"))
            {
                ComputeScene(Scenario2Model);
            }
            else if (Input.GetKeyDown("f"))
            {
                ComputeScene(Scenario3Model);
            }
            else
            {
                //Other or if user presses multiple keys
                ;
            }
        }
    }

    private bool additive = false;
    //public bool additive = false;
    private Scene previousScene;
    private int sceneLoadedCount = 0;

    //private void RemoveScenes()
    private void ComputeScene(string currentSceneName)
    {
        if (sceneLoadedCount == 0)
        {
            /// Add Initial Scene
            AddScene(ExistingConditionsModel);
            Debug.Log("Loaded Scene: " + ExistingConditionsModel);

            /// Add Demolition Model
            AddScene(currentSceneName);
            previousScene = SceneManager.GetSceneByName(currentSceneName);
            Debug.Log("Loading Scene: " + currentSceneName);
            previousScene = SceneManager.GetSceneByName(currentSceneName);
            sceneLoadedCount++;
        }

        else if (sceneLoadedCount > 0)
        {
            if (currentSceneName != previousScene.name)
            {
                /// Begin Adding Additional Scenes;
                if (additive == false)
                {
                    Debug.Log("Unloading Scene: " + previousScene);
                    // Unload Previous Scene
                    SceneManager.UnloadSceneAsync(previousScene);
                    //Destroy Previous Scene Game Object
                    RemoveScene(previousScene);
                }
                //TODO Geof7015 add Unload Scene Operation Here!
                Debug.Log("Loading Scene: " + currentSceneName);
                AddScene(currentSceneName);
                previousScene = SceneManager.GetSceneByName(currentSceneName);
                Debug.Log(previousScene.name);
                sceneLoadedCount++;
            }
        }
    }

    // Add Scene as Game Object
    private void AddScene(string currentSceneName)
    {
        // Modify Scene Position. Remember Unity is "Left Hand Coord System" and Y is Up.
        xPos += 0;
        yPos += 0;
        zPos += 0;

        // Call SceneLoader Script for loading Scene
        var sceneLoader = new GameObject(currentSceneName).AddComponent<SceneLoader>();
        sceneLoader.transform.position = new Vector3(0f, yPos, 0f);
        
        sceneLoader.Load(currentSceneName);
        if (DisableDirectionalLights)
        {
            _DisableDirectionalLights();
        }
    }

    // Disable Directional Lights
    private void _DisableDirectionalLights()
    {
        Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];
        foreach (Light light in lights)
        {   
            light.enabled = false;
        }
    }

// Remove Unity Scenes and Game Object
private void RemoveScene(Scene scene2Remove)
    {
        SceneManager.UnloadSceneAsync(scene2Remove);
        Debug.Log("Successfully Removed Scene Name : " + scene2Remove.name);
        GameObject gameObj2Remove = GameObject.Find(scene2Remove.name);
        if (gameObj2Remove != null)
        {
            gameObj2Remove.SetActive(false);
            Destroy(gameObj2Remove, 1.0f);
            print("It Does Exist : " + scene2Remove.name);

        }
    }
}

