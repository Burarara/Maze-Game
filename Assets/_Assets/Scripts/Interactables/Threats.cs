using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Threats : Interactable
{
    public enum WeaknessType
    {
        Bow,
        Sword,
        Magic
    }

    [SerializeField] private WeaknessType Weakness;
    
    protected override void OnEnterRange()
    {
        GameplayManager.Instance.BeginCombat(this);
    }

    public void OnDamage(WeaknessType damageType)
    {
        if (damageType != Weakness)
        {
            //Do something if damage does not correlate
            return;
        }
        GameplayManager.Instance.EndCombat();
        Destroy(gameObject);
    }
}
