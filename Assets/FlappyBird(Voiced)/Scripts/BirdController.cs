using UnityEngine;

public class BirdController : MonoBehaviour
{
    public float flapForce = 5f;
    private Rigidbody2D rb;
    private bool isDead = false;

    // Voice control settings
    public float loudnessThreshold = 0.1f; // Adjust this for sensitivity
    private AudioClip micClip;
    private string micName;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

#if UNITY_ANDROID && !UNITY_EDITOR
        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
            micClip = Microphone.Start(micName, true, 1, 44100);
        }
        else
        {
            Debug.LogWarning("No microphone found on device!");
        }
#endif
    }

    void Update()
    {
        if (isDead) return;

        // Keyboard (Editor) or Touch
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Flap();
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        // Voice control
        if (micClip != null && GetLoudnessFromMic() > loudnessThreshold)
        {
            Flap();
        }
#endif
    }

    void Flap()
    {
        rb.velocity = Vector2.up * flapForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Bird collided with: " + collision.gameObject.name);
        isDead = true;
        rb.velocity = Vector2.zero; // stop bird movement
        // TODO: Trigger game over screen or restart logic here
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    float GetLoudnessFromMic()
    {
        int micPosition = Microphone.GetPosition(micName) - 128;
        if (micPosition < 0) return 0;

        float[] waveData = new float[128];
        micClip.GetData(waveData, micPosition);

        float totalLoudness = 0;
        foreach (float sample in waveData)
        {
            totalLoudness += Mathf.Abs(sample);
        }
        return totalLoudness / 128;
    }
#endif
}
