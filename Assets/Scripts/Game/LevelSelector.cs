using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour
{
    public void LoadLevel(int num)
    {
        DeliveryGame.instance.LoadLevel(num);
    }
}
