using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BirdController : MonoBehaviour
{
    public float flapForce = 5f;
    private Rigidbody2D rb;
    private bool isDead = true; // Start paused

    // Score
    private int score = 0;
    public TextMeshProUGUI scoreText;

    // UI
    public Button startButton;

    private Vector3 startPosition;

    // Voice Control (Optional)
    public float loudnessThreshold = 0.1f;
    private AudioClip micClip;
    private string micName;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;

        rb.simulated = false;
        UpdateScoreText();

        startButton.gameObject.SetActive(true);
        startButton.onClick.AddListener(StartGame);

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

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Flap();
        }

#if UNITY_ANDROID && !UNITY_EDITOR
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
        Die();
    }

    void Die()
    {
        isDead = true;
        rb.simulated = false;
        rb.velocity = Vector2.zero;

        startButton.gameObject.SetActive(true);

        // Destroy all existing pipes
        foreach (var pipe in GameObject.FindGameObjectsWithTag("Pipe"))
        {
            Destroy(pipe);
        }

        // Stop pipe spawning
        FindObjectOfType<PipeSpawner>().StopSpawning();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "" + score.ToString();
    }

    public void StartGame()
    {
        // Reset bird
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        rb.simulated = true;

        // Reset state
        isDead = false;
        score = 0;
        UpdateScoreText();

        // Hide button
        startButton.gameObject.SetActive(false);

        // Start spawning pipes
        FindObjectOfType<PipeSpawner>().StartSpawning();
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
