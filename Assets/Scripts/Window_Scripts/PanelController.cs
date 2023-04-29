using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PanelController : MonoBehaviour
{
    [SerializeField] GameObject mousePos;
    public event Action Drag;

    public void WindowMove()
    {
        transform.position = mousePos.transform.position;
        Drag.Invoke();
    }
}
