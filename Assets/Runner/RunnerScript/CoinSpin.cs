using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    public float coinspeed = 100f;
    public AudioSource CoinSounds;

    private MeshRenderer meshRenderer;
    private Collider coinCollider;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        coinCollider = GetComponent<Collider>();
    }

    void Update()
    {
        transform.Rotate(Vector3.up * coinspeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance?.AddCoin();
            if (CoinSounds != null)
            {
                CoinSounds.Play();
            }

            if (meshRenderer != null)
                meshRenderer.enabled = false;

            if (coinCollider != null)
                coinCollider.enabled = false;

            Destroy(gameObject, CoinSounds != null ? CoinSounds.clip.length : 0.1f);
        }
    }

    public void ResetCoin()
    {
        // Reacquire components if they're null
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        if (coinCollider == null)
            coinCollider = GetComponent<Collider>();

        // Reset coin visibility and interactivity
        if (meshRenderer != null)
            meshRenderer.enabled = true;

        if (coinCollider != null)
            coinCollider.enabled = true;
    }
}
