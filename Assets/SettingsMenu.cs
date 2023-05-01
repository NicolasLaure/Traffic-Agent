using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer MusicMixer;
    public AudioMixer soundMixer;

    public int[] resolutionWidths;
    public int[] resolutionHeights;

    private bool isFullscreen;

    private void Start()
    {
        //Screen.SetResolution(1920, 1080, true);

    }

    public void OnVolumeChangeMusic(float value)
    {
        MusicMixer.SetFloat("music", value);
    }

    public void OnVolumeChangeSound(float value)
    {
        soundMixer.SetFloat("sound", value);
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        this.isFullscreen = isFullscreen;
    }

    public void SetResolution (int resolutionIndex)
    {
        Screen.SetResolution(resolutionWidths[resolutionIndex], resolutionHeights[resolutionIndex], isFullscreen);
    }
}
