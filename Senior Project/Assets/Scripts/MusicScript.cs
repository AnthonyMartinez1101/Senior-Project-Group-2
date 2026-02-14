using System.Collections;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    private AudioSource audioSource;


    public AudioClip dayMusic;

    [Range(0f, 2f)]
    public float musicVolume = 1f;

    bool isDayTime = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;

        audioSource.volume = musicVolume;

        PlayDayMusic();
    }

    void PlayDayMusic()
    {
        StartCoroutine(WaitFive());
    }

    IEnumerator WaitFive()
    {
        yield return new WaitForSeconds(5f);
        audioSource.clip = dayMusic;
        audioSource.volume = musicVolume;
        audioSource.Play();
    }

    public void ToggleDay()
    {
        StopAllCoroutines();
        isDayTime = !isDayTime;
        if (isDayTime) PlayDayMusic();
    }
}
