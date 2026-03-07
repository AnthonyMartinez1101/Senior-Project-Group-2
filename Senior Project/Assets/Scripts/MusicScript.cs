using System.Collections;
using UnityEngine;

public class MusicScript : MonoBehaviour
{
    private AudioSource audioSource;

    public WorldClock worldClock;


    public AudioClip dayMusic;

    [Range(0f, 2f)]
    public float musicVolume = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;

        audioSource.volume = musicVolume;

        if(!worldClock) GameManager.Instance.GetWorldClock();

        ToggleDay();
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
        if (worldClock.IsDay()) PlayDayMusic();
        else
        {
            //PlayNightMusic(); 
        }
    }
}
