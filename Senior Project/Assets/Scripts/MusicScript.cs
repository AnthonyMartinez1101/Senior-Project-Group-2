using System.Collections;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    public static MusicScript Instance;

    private AudioSource audioSource;

    public WorldClock worldClock;


    public AudioClip dayMusic;
    public AudioClip nightMusic;
    public AudioClip bossMusic;

    [Range(0f, 2f)]
    public float dayMusicVolume = 1f;

    [Range(0f, 2f)]
    public float nightMusicVolume = 1f;

    [Range(0f, 2f)]
    public float bossMusicVolume = 1f;

    public bool playBossMusic = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;

        audioSource.volume = dayMusicVolume;

        if(!worldClock) GameManager.Instance.GetWorldClock();

        if(playBossMusic) PlayBossMusic();
        else ToggleDay();
    }

    IEnumerator WaitFive()
    {
        yield return new WaitForSeconds(5f);
        LoadMusic(dayMusic, dayMusicVolume);
    }

    public void ToggleDay()
    {
        StopAllCoroutines();
        audioSource.Stop();
        if (worldClock.IsDay()) StartCoroutine(WaitFive());
        else
        {
            LoadMusic(nightMusic, nightMusicVolume);
        }
    }

    public void PlayBossMusic()
    {
        LoadMusic(bossMusic, bossMusicVolume);
    }

    private void LoadMusic(AudioClip audio, float volume)
    {
        audioSource.clip = audio;
        audioSource.volume = volume;
        audioSource.Play();
    }
}
