using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class WindowController : MonoBehaviour
{
    public event Action OnDrag;

    GameObject MousePointer;

    float xAxisOffset;
    float yAxisOffset;

    private void Awake()
    {
        MousePointer = GameObject.Find("MousePointer");
    }
    public void WindowMove()
    {
        Vector2 mousePos = MousePointer.transform.position;

        Vector2 offset = new Vector2(xAxisOffset, yAxisOffset);
        transform.position = mousePos + offset;

        OnDrag.Invoke();
    }

    public void XAxisOffset()
    {
        xAxisOffset = transform.position.x - MousePointer.transform.position.x;
    }
    public void YAxisOffset()
    {
        yAxisOffset = transform.position.y - MousePointer.transform.position.y;
    }

    public void DestroyWindow(GameObject window)
    {
        Destroy(window);
    }
}
