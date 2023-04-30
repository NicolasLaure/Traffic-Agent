using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager _instance;
    [SerializeField] private List<SoundFile> sounds = new List<SoundFile>();
    [SerializeField] private AudioSource[] SFXSources;
    [SerializeField] private bool generateArray = false;

    public AudioSource Music;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            PlayAudioClip(SoundCases.KeyboardPress);
        if (Input.GetKeyDown(KeyCode.D))
            PlayAudioClip(SoundCases.Error);
    }

    private void OnValidate()
    {
        if (generateArray)
        {
            sounds.Clear();
            for (int i = 0; i < (int)SoundCases.Count; i++)
            {
                sounds.Add(new SoundFile((SoundCases)i, null));
            }
            generateArray = false;
        }
        else
            return;
    }

    public bool PlayAudioClip(SoundCases soundCase, bool loop = false)
    {
        AudioClip playable = null;

        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].soundCase == soundCase)
                playable = sounds[i].audioClip;
        }

        if (playable == null)
            return false;
        else
        {
            bool played = false;

            for (int i = 0; i < SFXSources.Length; i++)
            {
                if(!SFXSources[i].isPlaying && !played)
                {
                    played = true;
                    if (loop)
                        SFXSources[i].loop = true;

                    SFXSources[i].clip = playable;
                    SFXSources[i].Play();
                }
            }
            return true;
        }
    }


}
public enum SoundCases
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
public struct SoundFile
{
    public SoundCases soundCase;
    public AudioClip audioClip;

    public SoundFile(SoundCases soundCase, AudioClip audioClip)
    {
        this.soundCase = soundCase;
        this.audioClip = audioClip;
    }
}