using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public void WindowInstantiate(GameObject windowPrefab)
    {
        if (!windowPrefab.CompareTag("GameWindow"))
        {
        GameObject newWindow =  Instantiate(windowPrefab, GameObject.Find("Canvas").transform);

        }
        else
        {
            GameObject gameWindow = GameObject.FindGameObjectWithTag("GameWindow");
            if (gameWindow == null)
            {
                gameWindow = Instantiate(windowPrefab, GameObject.Find("Canvas").transform);
            }
        }

        //Instantiate()
        //newWindow.transform.SetParent(GameObject.Find("Canvas").transform);
    }
}
