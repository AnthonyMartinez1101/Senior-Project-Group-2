using UnityEngine;

public class PlantAudio : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip[] rustlingClips;

    [Range(0f, 2f)]
    public float rustlingVolume = 1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Interact") || !audioSource.enabled) return;
        PlayRandom(rustlingClips, rustlingVolume, true);
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
