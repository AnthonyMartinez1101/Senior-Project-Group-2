using UnityEngine;

public class ExplosionAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] explosionSounds;

    public float explosionVolume = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource || explosionSounds == null || explosionSounds.Length == 0) return;

        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.PlayOneShot(explosionSounds[Random.Range(0, explosionSounds.Length)], explosionVolume);
    }
}
