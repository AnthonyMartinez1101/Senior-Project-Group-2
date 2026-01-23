using UnityEngine;

public class ShopAudio : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip[] buyCoinClips;
    public AudioClip[] sellCoinClips;

    [Range(0f, 1f)]
    public float buyCoinVolume = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBuyCoin()
    {
        PlayRandom(buyCoinClips, buyCoinVolume, true);
    }

    public void PlaySellCoin()
    {
        PlayRandom(sellCoinClips, buyCoinVolume, true);
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
