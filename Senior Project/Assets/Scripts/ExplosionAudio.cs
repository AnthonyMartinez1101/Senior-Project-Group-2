using UnityEngine;

public class ExplosionAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip explosionSound;

    public float explosionVolume = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) return;
        audioSource.pitch = Random.Range(0.5f, 1.5f);
        audioSource.PlayOneShot(explosionSound, explosionVolume);
    }
}
