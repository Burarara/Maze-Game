using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Since the treasures and threats will both be interactable it is best that they derive from each other in order to reuse as much code as possible.
/// This will reduce redundancy and help to keep code clean.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        OnEnterRange();
    }

    //The treasure and threats will want to implement their own version of this
    protected abstract void OnEnterRange();
}
