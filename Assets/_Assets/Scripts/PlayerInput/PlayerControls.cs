using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    public void OnMoveHorizontal(InputValue value)
    {
        var input = value.Get<Vector2>();
        if (input != Vector2.zero)
        {
            
        }
    }

    public void OnMoveVertical(InputValue value)
    {
        var input = value.Get<Vector2>();
        if (input != Vector2.zero)
        {
            
        }
    }
}
