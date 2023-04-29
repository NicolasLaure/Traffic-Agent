using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    [SerializeField] private List<SoundFile> sounds = new List<SoundFile>();
    [SerializeField] private bool generateArray = false;

    void Awake()
    {

        if (_instance == null)
        {

            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnValidate()
    {
        if (generateArray)
        {
            sounds.Clear();
            for (int i = 0; i < (int)SoundCases.Count; i++)
            {
                sounds.Add(new SoundFile( (SoundCases)i, null ));
            }
            generateArray = false;
        }  
        else
            return;
    }

}
enum SoundCases
{
    ClickError,
    Error,
    Error2,
    MotorbikeHit,
    MotorbikeEngine,
    Notification,
    PickUpPoint,
    PickUpPoint2,
    PickUpPoint3,
    PickUpPoint4,
    PopUp,
    Click,
    KeyboardPress,
    KeyboardPress2,
    KeyboardPress3,
    LoadingOS,
    StartUpOS,
    Count
}

[System.Serializable]
struct SoundFile
{
    public SoundCases soundCase;
    public AudioClip audioClip;

    public SoundFile(SoundCases soundCase, AudioClip audioClip)
    {
        this.soundCase = soundCase;
        this.audioClip = audioClip;
    }
}