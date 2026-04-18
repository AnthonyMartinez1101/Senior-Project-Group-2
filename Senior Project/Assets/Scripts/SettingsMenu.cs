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
    [SerializeField] private Toggle cameraToggle;
    
    private const string ShakeKey = "settings.shake";

    private const string MusicVolumeKey = "MusicVolume";
    private const string AmbientVolumeKey = "AmbientVolume";

    private const string MusicVolumeParam = "MusicVolume";
    private const string AmbientVolumeParam = "AmbientVolume";

    private readonly List<Vector2Int> resolutions = new()
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
        new Vector2Int(3840, 2160)

    };

    public void Start()
    {
        AddResolution();
    }
    private void OnEnable()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float ambientVolume = PlayerPrefs.GetFloat(AmbientVolumeKey, 1f);

        if (cameraToggle == null) return;
        bool on = PlayerPrefs.GetInt(ShakeKey, 1) == 1;
        cameraToggle.SetIsOnWithoutNotify(on);

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

    private void AddResolution()
    {
        resolutionDropdown.ClearOptions();

        var options = new List<string>();
        int curr = 0;

        for(int index = 0; index < resolutions.Count; index++)
        {
            var res = resolutions[index];
            options.Add($"{res.x} x {res.y}");

            if(Screen.width == res.x && Screen.height == res.y)
            {
                curr = index;
            }


        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = curr;
        resolutionDropdown.RefreshShownValue();
    }
    public void SetResolution(int index) 
    {
        Vector2Int r = resolutions[index];
        Screen.SetResolution(r.x, r.y, Screen.fullScreen);
    }

    public void SetCameraShake(bool on)
    {
        //CameraShake(on);
        PlayerPrefs.SetInt(ShakeKey, on ? 1 : 0); //shake on is 1, off is 0
        PlayerPrefs.Save();

        var shake = FindAnyObjectByType<CameraShake>();
        if (shake != null)
        {
            shake.enabled = on;
        }
    }

}

// updates the sliders when the menu is opened
