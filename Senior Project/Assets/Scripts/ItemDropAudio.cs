using UnityEngine;

public class ItemDropAudio : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip[] dropClips;

    [Range(0f, 2f)]
    public float dropVolume = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDropSound()
    {
        PlayRandom(dropClips, dropVolume, true);
        dropVolume -= 0.2f;
    }

    private void PlayRandom(AudioClip[] clips, float volume, bool randomPitch)
    {
        if (audioSource == null || clips == null || clips.Length == 0) return;
        int index = Random.Range(0, clips.Length);

        if (randomPitch) audioSource.pitch = Random.Range(0.8f, 1.2f);
        else audioSource.pitch = 1f;

        audioSource.PlayOneShot(clips[index], volume);
    }
}
