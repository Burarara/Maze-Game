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
    [SerializeField] private GameObject CombatUI;
    [SerializeField] private TMP_Text TreasureCountUI;
    
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

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

#endregion
}
