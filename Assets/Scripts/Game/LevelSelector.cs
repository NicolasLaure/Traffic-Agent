using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public void LoadLevel(int num)
    {
        if (num < GameManager.Instance.levelsUnlocked)
        {
            DeliveryGame.instance.LoadLevel(num);
        }
    }
}
