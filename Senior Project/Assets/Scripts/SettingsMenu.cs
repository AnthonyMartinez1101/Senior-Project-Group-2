using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider ambientSlider;

    private const string MusicVolumeKey = "MusicVolume";
    private const string AmbientVolumeKey = "AmbientVolume";

    private const string MusicVolumeParam = "MusicVolume";
    private const string AmbientVolumeParam = "AmbientVolume";

    private void OnEnable()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float ambientVolume = PlayerPrefs.GetFloat(AmbientVolumeKey, 1f);

        if(musicSlider != null)
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
}

// updates the sliders when the menu is opened
