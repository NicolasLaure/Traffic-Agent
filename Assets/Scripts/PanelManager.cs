using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public void WindowInstantiate(GameObject windowPrefab)
    {

        GameObject newWindow =  Instantiate(windowPrefab, GameObject.Find("Canvas").transform);
        //Instantiate()
        //newWindow.transform.SetParent(GameObject.Find("Canvas").transform);
    }
}
