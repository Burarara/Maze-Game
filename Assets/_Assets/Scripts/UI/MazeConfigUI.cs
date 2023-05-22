using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This is a class that will display the configuration UI. Will be assigned when the configuration UI is updated.
/// </summary>
public class MazeConfigUI : MonoBehaviour
{
    public static MazeConfigUI Instance;

    [Header("UI - Display")]
    [SerializeField] private TMP_Text ConfigButtonText;
    //Current config stats to display
    [SerializeField] private TMP_Text RoomsDisplay;
    [SerializeField] private TMP_Text ThreatsDisplay;
    [SerializeField] private TMP_Text TreasureDisplay;

    [Header("UI - User Input")] 
    [SerializeField] private TMP_InputField UserRoomsInput;
    [SerializeField] private TMP_InputField UserThreatsInput;
    [SerializeField] private TMP_InputField UserTreasureInput;

    private bool isUserFieldActive;

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

    public void UpdateUI(MazeConfiguration newConfig)
    {
        RoomsDisplay.text = newConfig.Rooms.ToString();
        ThreatsDisplay.text = newConfig.Threats.ToString();
        TreasureDisplay.text = newConfig.Treasures.ToString();
    }

#region Button Callback

    public void ToggleConfig()
    {
        if (!isUserFieldActive)
        {
            EnableUserInput();
            ConfigButtonText.text = "SAVE";
        }
        else
        {
            DisableUserInput();
            Tuple<bool, MazeConfiguration> result = TryParseUserInput();
            if (result.Item1)
            {
                ConfigurationHandler.Instance.SaveConfig(result.Item2);
                UpdateUI(result.Item2);
            }

            ConfigButtonText.text = "CONFIGURE MAZE";
        }
    }

    public void EnableUserInput()
    {
        isUserFieldActive = true;
        UserRoomsInput.transform.parent.gameObject.SetActive(true);
        UserThreatsInput.transform.parent.gameObject.SetActive(true);
        UserTreasureInput.transform.parent.gameObject.SetActive(true);
        
        UserRoomsInput.text = "...";
        UserThreatsInput.text = "...";
        UserTreasureInput.text = "...";
    }

    public void DisableUserInput()
    {
        isUserFieldActive = false;
        UserRoomsInput.transform.parent.gameObject.SetActive(false);
        UserThreatsInput.transform.parent.gameObject.SetActive(false);
        UserTreasureInput.transform.parent.gameObject.SetActive(false);
    }

    /// <summary>
    /// Attempt to parse the current user input, if it does not parse correctly then we do not update the UI and do not update the configuration handler
    /// Return a bool and MazeConfiguration tuple to return success and the new input values. Returns the current config on a failure. 
    /// </summary>
    /// <returns></returns>
    public Tuple<bool, MazeConfiguration> TryParseUserInput()
    {
        if (!int.TryParse(UserRoomsInput.text, out int newRooms))
        {
            return new Tuple<bool, MazeConfiguration>(false, ConfigurationHandler.Instance.CurrentConfig);
        }
        if (!int.TryParse(UserThreatsInput.text, out int newThreats))
        {
            return new Tuple<bool, MazeConfiguration>(false, ConfigurationHandler.Instance.CurrentConfig);
        }
        if (!int.TryParse(UserTreasureInput.text, out int newTreasureInput))
        {
            return new Tuple<bool, MazeConfiguration>(false, ConfigurationHandler.Instance.CurrentConfig);
        }

        return new Tuple<bool, MazeConfiguration>(true, new MazeConfiguration(newRooms, newThreats, newTreasureInput));
    }

#endregion
}
