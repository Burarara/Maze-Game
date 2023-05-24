using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitTile : Interactable
{
    protected override void OnEnterRange()
    {
        GameplayManager.Instance.BeginGameOverUI();
    }
}
