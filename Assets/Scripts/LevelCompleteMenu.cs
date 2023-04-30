using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteMenu : MonoBehaviour
{
    public void LoadNextLevel()
    {
        DeliveryGame.instance.LoadNextLevel();
    }

    public void LoadMainMenu()
    {
        DeliveryGame.instance.LoadMainMenu();
    }
}
