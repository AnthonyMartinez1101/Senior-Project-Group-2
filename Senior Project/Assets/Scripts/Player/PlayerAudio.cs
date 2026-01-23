using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip[] footstepClips;
    public AudioClip[] scytheSwingClips;
    public AudioClip[] munchClips;
    public AudioClip[] wateringClips;
    public AudioClip[] waterRefillClips;
    public AudioClip[] inventoryPopClips;

    [Header("Volume")]
    [Range(0f, 2f)]
    public float footstepVolume = 1f;

    [Range(0f, 2f)]
    public float scytheSwingVolume = 1f;

    [Range(0f, 2f)]
    public float munchVolume = 1f;

    [Range(0f, 2f)]
    public float wateringVolume = 1f;

    [Range(0f, 2f)]
    public float waterRefillVolume = 1f;

    [Range(0f, 2f)]
    public float inventoryPopVolume = 1f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFootstep()
    {
        PlayRandom(footstepClips, footstepVolume, true);
    }

    public void PlayScytheSwing()
    {
        PlayRandom(scytheSwingClips, scytheSwingVolume, true);
    }

    public void PlayMunch()
    {
        PlayRandom(munchClips, munchVolume, true);
    }

    public void PlayWatering()
    {
        PlayRandom(wateringClips, wateringVolume, true);
    }

    public void PlayWaterRefill()
    {
        PlayRandom(waterRefillClips, waterRefillVolume, true);
    }

    public void PlayInventoryPop()
    {
        PlayRandom(inventoryPopClips, inventoryPopVolume, true);
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
