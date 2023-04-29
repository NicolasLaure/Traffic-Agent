using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelDrag : MonoBehaviour
{
    private void Start()
    {
        transform.parent.GetComponent<PanelController>().Drag += MovePanel;
    }

    private void MovePanel()
    {
        Debug.Log("me muevo");
        //  transform.parent.position = 
    }

}
