using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    public void LoadPreviousLevel()
    {
        DeliveryGame.instance.LoadPreviousLevel();
    }

    public void LoadMainMenu()
    {
        DeliveryGame.instance.LoadMainMenu();
    }
}
