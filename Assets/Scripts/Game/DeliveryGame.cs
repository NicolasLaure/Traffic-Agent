using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryGame : MonoBehaviour
{
    public static DeliveryGame instance;

    public GameObject mainMenu;
    public GameObject levelSelect;
    public GameObject gameOver;
    public GameObject levelComplete;
    public GameObject[] levels;

    [HideInInspector]
    public int winCondition = -1;
    [HideInInspector]
    public int engineSound = 0;

    GameObject currentlyLoaded;
    int previousLevel;

    // Start is called before the first frame update
    void Start()
    {
        engineSound = 0;
        instance = this;
        LoadMainMenu();
    }

    public void AddEngine()
    {
        if (engineSound < 1)
        {
            SoundManager._instance?.PlayAudioClip(SoundCases.MotorbikeEngine, true);
        }
        engineSound++;
    }

    public void RemoveEngine()
    {
        engineSound--;
        if (engineSound < 1)
        {
            StopEngineSound();
        }
    }

    public void WindowDestroyed()
    {
        DeliveryGame.instance.CloseGame();
    }

    public void CloseGame()
    {
        StopEngineSound();
        GameObject.FindGameObjectWithTag("AdGenerator").GetComponent<AdGeneratorController>().EndLevel();
    }

    public void StopEngineSound()
    {
        engineSound = 0;
        SoundManager._instance?.StopAudioClip(SoundCases.MotorbikeEngine);
    }

    public void EndGame()
    {
        StopEngineSound();
        if (winCondition == 0)
        {
            LoadLevelComplete();
        }
        else
        {
            LoadGameOver();
        }
    }

    public void LoadMainMenu()
    {
        Load(mainMenu);
    }

    public void LoadLevelSelect()
    {
        Load(levelSelect);
    }

    public void LoadGameOver()
    {
        Load(gameOver);
    }

    public void LoadLevelComplete()
    {
        Load(levelComplete);
    }

    public void LoadLevel(int num)
    {
        previousLevel = num;
        Load(levels[num]);
    }

    public void LoadPreviousLevel()
    {
        LoadLevel(previousLevel);
    }

    public void LoadNextLevel()
    {
        if (previousLevel + 1 < levels.Length)
        {
            LoadLevel(previousLevel + 1);
        }
        else
        {
            //TODO: END OF THE GAME!
            Debug.LogError("YOU WIN!!!");
        }
    }

    void Load(GameObject go)
    {
        Destroy(currentlyLoaded);
        currentlyLoaded = Instantiate(go, gameObject.transform);
    }

#if UNITY_EDITOR
    //This weird workaround is to avoid a spam warning
    void OnValidate() { UnityEditor.EditorApplication.delayCall += _OnValidate; }

    //Whenever this script is loaded, or a variable is changed, the following function is called
    void _OnValidate()
    {
        if (this == null) return;
        if (Mathf.Approximately((float)(gameObject.GetComponent<RectTransform>().rect.width / 1.252), (float)(gameObject.GetComponent<RectTransform>().rect.height / 0.794)) == false)
        {
            Debug.LogError("Error: Game panel does not have the ratio of 1252 x 794");
            Debug.Log((float)(gameObject.GetComponent<RectTransform>().rect.width / 1.252));
            Debug.Log((float)(gameObject.GetComponent<RectTransform>().rect.height / 0.794));
        }
        else
        {
            //Debug.Log("Congratulations, the game panel has the correct ratio of 1252 x 794");
        }
    }
#endif
}