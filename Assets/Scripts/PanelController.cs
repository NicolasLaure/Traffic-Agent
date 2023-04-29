using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class PanelController : MonoBehaviour
{
    public event Action Drag;

    public void WindowMove()
    {
        transform.position = Input.mousePosition;
        Drag.Invoke();
    }
}
