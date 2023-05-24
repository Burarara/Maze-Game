using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasures : Interactable
{
    protected override void OnEnterRange()
    {
        GameplayManager.Instance.UpdateTreasureCount();
        Destroy(gameObject);
    }
}
