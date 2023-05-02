using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabController : MonoBehaviour
{
    [SerializeField] GameObject panel;

    private void Start()
    {
        if (gameObject.CompareTag("Tab"))
            panel = transform.GetChild(1).gameObject;
    }

    public void PanelSwitch()
    {
        SoundManager._instance?.PlayAudioClip(SoundCases.Click);
        if (gameObject.CompareTag("Tab"))
        {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("SystemPanel"))
            {
                if (gameObject != this)
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
        else
        {
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
}
