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
    private string SAVE_FILE;
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
        
        SAVE_FILE = Application.persistentDataPath + "/ConfigurationSave.txt";
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

        StreamWriter writer = new StreamWriter(SAVE_FILE, false);
        writer.WriteLine(saveData);
        writer.Close();
        
        MazeConfigUtils.CurrentConfig = newConfig;
    }

    public MazeConfiguration LoadConfig()
    {
        if (!File.Exists(SAVE_FILE))
        {
            File.WriteAllText(SAVE_FILE, JsonUtility.ToJson(new MazeConfiguration(10, 4, 6)));
        }

        StreamReader reader = new StreamReader(SAVE_FILE);
        string loadString = reader.ReadToEnd();
        reader.Close();
        
        MazeConfiguration loadConfig;
        try
        {
            loadConfig = JsonUtility.FromJson<MazeConfiguration>(loadString);
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
