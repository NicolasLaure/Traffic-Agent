using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TitleBarController : MonoBehaviour
{
    private void Start()
    {
        transform.parent.GetComponent<WindowController>().OnDrag += MovePanel;
    }

    private void MovePanel()
    {

    }

}
