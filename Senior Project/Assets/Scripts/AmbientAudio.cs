using System.Collections;
using UnityEngine;

public class AmbientAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] dayAmbientClips;
    public AudioClip[] nightAmbientClips;

    private Coroutine ambientCoroutine;

    bool isDaytime = true;

    [Range(0f, 2f)]
    public float ambientVolume = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartAmbiance();
    }

    private void StartAmbiance()
    {
        if(ambientCoroutine != null) StopCoroutine(ambientCoroutine);
        ambientCoroutine = StartCoroutine(PlayLoop());
    }

    IEnumerator PlayLoop()
    {
        while (true)
        {
            if(isDaytime) PlayRandom(dayAmbientClips, ambientVolume);
            else PlayRandom(nightAmbientClips, ambientVolume);

            while(audioSource.isPlaying)
            {
                yield return null;
            }
        }
    }

    private void PlayRandom(AudioClip[] clips, float volume)
    {
        if (audioSource == null || clips == null || clips.Length == 0) return;
        int index = Random.Range(0, clips.Length);

        audioSource.clip = clips[index];
        audioSource.volume = volume;
        audioSource.Play();
    }

    public void ToggleDay()
    {
        isDaytime = !isDaytime;
        StartAmbiance();
    }
}
