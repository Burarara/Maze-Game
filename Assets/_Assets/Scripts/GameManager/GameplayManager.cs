using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance;
    private int _treasureCount;

    private Threats currentThreat;
    //UI
    [SerializeField] private GameObject PauseUI;
    [SerializeField] private GameObject ResumeUI;
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private TMP_Text FinalTreasureCountUI;
    [SerializeField] private GameObject CombatUI;
    [SerializeField] private TMP_Text TreasureCountUI;

    public bool isGameOver;
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
    
    private void OnDestroy()
    {
        Instance = null;
    }

#region Update Gameplay Stats

    public void UpdateTreasureCount()
    {
        _treasureCount++;
        TreasureCountUI.text = _treasureCount.ToString();
    }

#endregion

#region Combat Button Callbacks

    public void BeginCombat(Threats newThreat)
    {
        PlayerControls.CanMove = false;
        CombatUI.SetActive(true);
        currentThreat = newThreat;
    }

    public void EndCombat()
    {
        PlayerControls.CanMove = true;
        CombatUI.SetActive(false);
        currentThreat = null;
    }

    public void BowCallback()
    {
        currentThreat.OnDamage(Threats.WeaknessType.Bow);
    }

    public void SwordCallback()
    {
        currentThreat.OnDamage(Threats.WeaknessType.Sword);
    }

    public void MagicCallback()
    {
        currentThreat.OnDamage(Threats.WeaknessType.Magic);
    }

#endregion
    
#region Menu Button Callbacks

    public void TogglePauseUI()
    {
        var newState = !PauseUI.gameObject.activeSelf;
        PauseUI.gameObject.SetActive(newState);
        PlayerControls.CanMove = !newState;
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void BeginGameOverUI()
    {
        PauseUI.SetActive(true);
        GameOverUI.SetActive(true);
        ResumeUI.SetActive(false);
        FinalTreasureCountUI.text = TreasureCountUI.text;
        PlayerControls.CanMove = false;
        TreasureCountUI.transform.parent.gameObject.SetActive(false);
    }

#endregion
}
