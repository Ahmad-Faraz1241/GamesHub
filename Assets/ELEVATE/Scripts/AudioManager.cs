using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;
    public AudioClip placeClip;
    public AudioClip gameOverClip;
    public AudioClip sliceClip;

    private void Awake()
    {
        Instance = this; // Always overwrite
    }

    public void PlayPlace()
    {
        audioSource.PlayOneShot(placeClip);
    }

    public void PlayGameOver()
    {
        audioSource.PlayOneShot(gameOverClip);
    }

    public void PlaySlice()
    {
        audioSource.PlayOneShot(sliceClip);
    }
}
