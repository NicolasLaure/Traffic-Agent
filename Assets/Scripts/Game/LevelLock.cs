using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelLock : MonoBehaviour
{
    public int levelNum;
    public Color tint;
    public GameObject lockImg;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.levelsUnlocked < levelNum)
        {
            gameObject.GetComponent<Image>().color = tint;
            lockImg.SetActive(true);
        }
    }
}
