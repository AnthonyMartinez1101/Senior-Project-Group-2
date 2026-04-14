using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    public CameraShake cameraShake;
    private const string ShakeKey = "Shake";

    private const string MusicVolumeKey = "MusicVolume";
    private const string AmbientVolumeKey = "AmbientVolume";

    private const string MusicVolumeParam = "MusicVolume";
    private const string AmbientVolumeParam = "AmbientVolume";

    private Resolution[] resolutions;


    public void Start()
    {
        resolutions = Screen.resolutions;

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) 
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    private void OnEnable()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float ambientVolume = PlayerPrefs.GetFloat(AmbientVolumeKey, 1f);

        bool on = PlayerPrefs.GetInt(ShakeKey, 1) == 1;
        CameraShake(on);

        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(musicVolume);
        }
        if(ambientSlider != null)
        {
            ambientSlider.SetValueWithoutNotify(ambientVolume);
        }

    }

    public void MusicSetting(float MV)
    {
        mixer.SetFloat(MusicVolumeParam, LinearToDb(MV));
        PlayerPrefs.SetFloat(MusicVolumeKey, MV);
        PlayerPrefs.Save();
    }

    public void AmbientSetting(float MV)
    {
        mixer.SetFloat(AmbientVolumeParam, LinearToDb(MV));
        PlayerPrefs.SetFloat(AmbientVolumeKey, MV);
        PlayerPrefs.Save();
    }

    public void FullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    private float LinearToDb(float v)
    {
        if(v <= 0.0001f) return -80f; // Minimum dB value for silence
        return Mathf.Log10(v) * 20f;
    }

    public void SetResolution(int resolutionIndex) 
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetCameraShake(bool on)
    {
        CameraShake(on);
        PlayerPrefs.SetInt(ShakeKey, on ? 1 : 0); //shake 
        PlayerPrefs.Save();
    }

    private void CameraShake(bool on) 
    {
        if (cameraShake != null)
        {
            cameraShake.enabled = on;
        }
    }
       
}

// updates the sliders when the menu is opened
