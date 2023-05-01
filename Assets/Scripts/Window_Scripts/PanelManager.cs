using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject windowMaster;

    public void WindowInstantiate(GameObject windowPrefab)
    {
        WindowInstantiate(windowPrefab, Vector3.zero);
    }

    public void WindowInstantiate(GameObject windowPrefab, Vector3 pos)
    {
        if (!windowPrefab.CompareTag("GameWindow"))
        {
            GameObject adGameObject = Instantiate(windowPrefab, windowMaster.transform);
            adGameObject.transform.position = pos;
        }
        else
        {
            GameObject gameWindow = GameObject.FindGameObjectWithTag("GameWindow");
            if (gameWindow == null)
            {
                Instantiate(windowPrefab, windowMaster.transform);
                
            }
        }
    }
}
