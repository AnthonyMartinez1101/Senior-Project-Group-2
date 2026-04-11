using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingApplier : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    private const string MusicVolumeKey = "MusicVolume";
    private const string AmbientVolumeKey = "AmbientVolume";

    private const string MusicVolumeParam = "MusicVolume";
    private const string AmbientVolumeParam = "AmbientVolume";

    private void Awake()
    {
        float music = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float ambient = PlayerPrefs.GetFloat(AmbientVolumeKey, 1f);
        mixer.SetFloat(MusicVolumeParam, LinearToDb(music));
        mixer.SetFloat(AmbientVolumeParam, LinearToDb(ambient));
    }

    private float LinearToDb(float v)
    {
        if (v <= 0.0001f) return -80f; // Minimum dB value for silence
        return Mathf.Log10(v) * 20f;
    }
}

// saves the games volume settings to player prefs and applies them to the audio mixer.
// i used awake bc of immedate application (but it could be start if we want to ensure the mixer is ready)