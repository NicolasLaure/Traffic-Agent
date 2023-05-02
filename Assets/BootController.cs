using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootController : MonoBehaviour
{

    [SerializeField] float bootTime;
    GameObject cursor;
    private void Awake()
    {
        gameObject.SetActive(true);
    }
    private void Start()
    {
        cursor = GameObject.Find("MousePointer");
        cursor.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        SoundManager._instance?.PlayAudioClip(SoundCases.LoadingOS);
        StartCoroutine(BootTime());

    }


    IEnumerator BootTime()
    {
        yield return new WaitForSeconds(bootTime);
        SoundManager._instance?.PlayAudioClip(SoundCases.StartUpOS);
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Confined;
        cursor.SetActive(true);
    }
}
