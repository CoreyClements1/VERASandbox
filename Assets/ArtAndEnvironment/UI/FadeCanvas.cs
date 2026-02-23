using UnityEngine;
internal class FadeCanvas : MonoBehaviour
{

    public static FadeCanvas Instance { get; private set; }

    [SerializeField] private CanvasGroup canvasGroup;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Fades the canvas in over the specified duration
    public void FadeIn(float duration)
    {
        StartCoroutine(FadeCoroutine(1f, duration));
    }

    // Fades the canvas out over the specified duration
    public void FadeOut(float duration)
    {
        StartCoroutine(FadeCoroutine(0f, duration));
    }

    // Fades the canvas to a specific alpha value over the specified duration
    public void FadeTo(float targetAlpha, float duration)
    {
        StartCoroutine(FadeCoroutine(targetAlpha, duration));
    }

    // Coroutine that handles the alpha fade over time
    private System.Collections.IEnumerator FadeCoroutine(float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}