using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabController : MonoBehaviour
{
    [SerializeField] GameObject panel;

    private void Start()
    {
        panel = transform.GetChild(1).gameObject;
    }

    public void PanelSwitch()
    {
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("SystemPanel"))
        {
            if(gameObject != this)
            {
                gameObject.SetActive(false);
            }
        }
        if (panel.activeInHierarchy)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
        }
    }
}
