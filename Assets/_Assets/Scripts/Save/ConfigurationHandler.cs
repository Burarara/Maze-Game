using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// This is a class to handle the saving and loading of the configuration file. We ensure that the 
/// </summary>
public class ConfigurationHandler : MonoBehaviour
{
    public TextAsset ConfigurationSave;

    public static ConfigurationHandler Instance;
    

#region MonoBehaviour
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        MazeConfiguration loadConfig = LoadConfig();
        MazeConfigUI.Instance.UpdateUI(loadConfig);
        MazeConfigUtils.CurrentConfig = loadConfig;
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }
    
#endregion

#region SaveLoad

    public void SaveConfig(MazeConfiguration newConfig)
    {
        string saveData = JsonUtility.ToJson(newConfig);
        File.WriteAllText(AssetDatabase.GetAssetPath(ConfigurationSave), saveData);
        MazeConfigUtils.CurrentConfig = newConfig;
    }

    public MazeConfiguration LoadConfig()
    {
        MazeConfiguration loadConfig;
        try
        {
            loadConfig = JsonUtility.FromJson<MazeConfiguration>(ConfigurationSave.text);
        }
        catch (Exception e)
        {
            loadConfig = GenerateRandomConfig();
            SaveConfig(loadConfig);
        }
        return loadConfig;
    }

#endregion

#region Utility

    public MazeConfiguration GenerateRandomConfig()
    {
        MazeConfiguration randomConfig = new MazeConfiguration();
        randomConfig.Rooms = Random.Range(5, 10);
        randomConfig.Treasures = Random.Range(1, randomConfig.Rooms);
        randomConfig.Threats = Random.Range(1, randomConfig.Rooms);
        return randomConfig;
    }

#endregion
}
