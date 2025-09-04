using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FruitSlicer3D : MonoBehaviour
{
    [Header("Settings")]
    public bool isFruit = true;            // Only true for fruits
    public GameObject half1Prefab;
    public GameObject half2Prefab;
    public GameObject splashPrefab;
    public float scatterForce = 5f;
    public float halfLifetime = 2f;

    [Header("Audio")]
    public AudioClip sliceSound;
    [Range(0f, 1f)] public float sliceVolume = 1f;

    [Header("Pooling")]
    public int splashPoolSize = 20;
    public int halfPoolSize = 20;

    protected bool sliced = false;

    private List<GameObject> splashPool;
    private List<GameObject> half1Pool;
    private List<GameObject> half2Pool;

    void Awake()
    {
        if (!isFruit) return; // Skip pool creation for bombs

        splashPool = CreatePool(splashPrefab, splashPoolSize);
        half1Pool = CreatePool(half1Prefab, halfPoolSize);
        half2Pool = CreatePool(half2Prefab, halfPoolSize);
    }

    private List<GameObject> CreatePool(GameObject prefab, int size)
    {
        List<GameObject> pool = new List<GameObject>();
        if (prefab == null) return pool;

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Add(obj);
        }
        return pool;
    }

    public virtual void Slice(Vector3 swipeDirection)
    {
        if (sliced) return;
        sliced = true;

        // --- Add Score ---
        if (isFruit)
        {
            ScoreManager3D scoreManager = FindObjectOfType<ScoreManager3D>();
            if (scoreManager != null)
                scoreManager.AddScore(1); // +1 per fruit
        }

        // --- Play slice sound ---
        if (sliceSound != null)
            AudioSource.PlayClipAtPoint(sliceSound, Camera.main.transform.position, sliceVolume);

        if (!isFruit) return; // Bombs do nothing else

        // --- Splash ---
        GameObject splash = GetFromPool(splashPool);
        if (splash != null)
        {
            splash.transform.position = transform.position;
            splash.transform.rotation = Quaternion.identity;
            splash.SetActive(true);

            SpriteRenderer sr = splash.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color; c.a = 1f; sr.color = c;
                StartCoroutine(FadeSplash(sr, 1.5f));
            }
            else
                StartCoroutine(DisableAfterTime(splash, 1.5f));
        }

        // --- Halves ---
        GameObject h1 = GetFromPool(half1Pool);
        GameObject h2 = GetFromPool(half2Pool);

        if (h1 != null)
        {
            h1.transform.position = transform.position;
            h1.transform.rotation = transform.rotation;
            h1.SetActive(true);
            AddPhysics(h1, (swipeDirection.normalized + Vector3.up) * scatterForce);
            StartCoroutine(DisableAfterTime(h1, halfLifetime));
        }

        if (h2 != null)
        {
            h2.transform.position = transform.position;
            h2.transform.rotation = transform.rotation;
            h2.SetActive(true);
            AddPhysics(h2, (swipeDirection.normalized + Vector3.down) * scatterForce);
            StartCoroutine(DisableAfterTime(h2, halfLifetime));
        }

        gameObject.SetActive(false); // Return whole fruit to pool
    }

    private GameObject GetFromPool(List<GameObject> pool)
    {
        if (pool == null || pool.Count == 0) return null;

        foreach (var obj in pool)
            if (!obj.activeInHierarchy) return obj;

        // Create new if pool exhausted
        GameObject newObj = Instantiate(pool[0]);
        newObj.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }

    private IEnumerator FadeSplash(SpriteRenderer sr, float duration)
    {
        float timer = 0f;
        Color c = sr.color;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, timer / duration);
            sr.color = c;
            yield return null;
        }
        sr.gameObject.SetActive(false);
    }

    private IEnumerator DisableAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        obj.SetActive(false);
    }

    private void AddPhysics(GameObject obj, Vector3 force)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null) return;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(force, ForceMode.Impulse);
    }

    public void ResetSliced() => sliced = false;
}
