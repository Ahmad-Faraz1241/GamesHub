using UnityEngine;
using System.Collections;

public class FruitSlicer3D : MonoBehaviour
{
    [Header("Slice Settings")]
    public GameObject half1Prefab;
    public GameObject half2Prefab;
    public GameObject splashPrefab;
    public float scatterForce = 5f;
    public float lifetime = 2f;

    public AudioClip sliceSound;
    [Range(0f, 1f)] public float sliceVolume = 1f;

    protected bool sliced = false; // protected so child classes can access

    public virtual void Slice(Vector3 swipeDirection)
    {
        if (sliced) return;
        sliced = true;

        // Play slice sound
        if (sliceSound != null)
            AudioSource.PlayClipAtPoint(sliceSound, Camera.main.transform.position, sliceVolume);

        // Spawn splash with fade
        if (splashPrefab != null)
        {
            GameObject splash = Instantiate(splashPrefab, transform.position, Quaternion.identity);
            SpriteRenderer sr = splash.GetComponent<SpriteRenderer>();

            if (sr != null)
                StartCoroutine(FadeSplash(sr, 1.5f)); // fade over 1.5 seconds
            else
                Destroy(splash, 1.5f); // fallback
        }

        // Spawn fruit halves
        GameObject h1 = Instantiate(half1Prefab, transform.position, transform.rotation);
        GameObject h2 = Instantiate(half2Prefab, transform.position, transform.rotation);

        AddPhysics(h1, (swipeDirection.normalized + Vector3.up) * scatterForce);
        AddPhysics(h2, (swipeDirection.normalized + Vector3.down) * scatterForce);

        Destroy(h1, lifetime);
        Destroy(h2, lifetime);

        // Disable whole fruit (back to pool)
        gameObject.SetActive(false);
    }

    // Coroutine for splash fading
    private IEnumerator FadeSplash(SpriteRenderer sr, float duration)
    {
        float timer = 0f;
        Color original = sr.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            sr.color = new Color(original.r, original.g, original.b, alpha);
            yield return null;
        }

        Destroy(sr.gameObject);
    }

    public void ResetSliced() => sliced = false;

    private void AddPhysics(GameObject obj, Vector3 force)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) rb = obj.AddComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(force, ForceMode.Impulse);
    }
}
