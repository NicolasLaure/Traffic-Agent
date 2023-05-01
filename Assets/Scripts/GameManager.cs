using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = (T)(object)this;
    }
}

public class GameManager : Singleton<GameManager>
{
    public int levelsUnlocked = 1;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

   public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Debug.Log("Salir del juego");
    }

    public void ResetLevels()
    {
        levelsUnlocked = 1;
    }
}
