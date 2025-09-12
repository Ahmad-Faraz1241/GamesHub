using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public Image fadeImage; // Reference to the FadePanel's Image
    public float fadeDuration = 0.5f;

    public static SceneTransitionManager Instance;

    void Awake()
    {
        // Singleton pattern — make sure only one exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }

    public void TransitionToScene(int sceneIndex)
    {
        StartCoroutine(FadeAndSwitch(sceneIndex));
    }

    IEnumerator FadeAndSwitch(int sceneIndex)
    {
        yield return StartCoroutine(Fade(1f)); // Fade to black

        // Start loading scene
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            yield return null;
        }

        op.allowSceneActivation = true;

        yield return null; // wait one frame

        yield return StartCoroutine(Fade(0f)); // Fade back in
    }

    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            time += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, targetAlpha);
    }
}
