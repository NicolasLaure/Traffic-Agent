using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconController : MonoBehaviour
{
    public int clickCount = 0;
    GameObject gameManager;
    [SerializeField] GameObject windowPrefab;
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager");
    }
    public void AddClick()
    {
        clickCount++;
        StartCoroutine(timer());

        if (clickCount >= 2)
        {
            gameManager.GetComponent<PanelManager>().WindowInstantiate(windowPrefab);
            clickCount = 0;
        }
    }

    IEnumerator timer()
    {
        yield return new WaitForSeconds(1);
        clickCount = 0;
    }
}
