using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    [SerializeField] private LayerMask wallLayer;
    private void Move(Vector3 moveDir)
    {
        transform.position += moveDir;
    }

    private void Flip(float dir)
    {
        transform.localScale = new Vector3(dir, 1f, 1f);
    }

#region ButtonCallbacks

    public void OnMoveHorizontal(InputValue value)
    {
        var input = value.Get<Vector2>();
        if (input != Vector2.zero && 
            !Physics.Raycast(transform.position, input, 1f, wallLayer))
        {
            Move(input);
            Flip(input.x);
        }
    }

    public void OnMoveVertical(InputValue value)
    {
        var input = value.Get<Vector2>();
        if (input != Vector2.zero && 
            !Physics.Raycast(transform.position, input, 1f, wallLayer))
        {
            Move(input);
        }
    }

#endregion
    
}
