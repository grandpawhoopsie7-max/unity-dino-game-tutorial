
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class SandstormEffect : MonoBehaviour
{
    [Header("Storm Timing")]
    public float minimumWait = 18f;
    public float maximumWait = 28f;

    [Header("Storm Appearance")]
    [Range(0f, 1f)]
    public float strongestAlpha = 0.30f;

    public float buildupDuration = 3f;
    public float strongestDuration = 8f;
    public float clearingDuration = 4f;

    [Header("Sandstorm Vignette")]
    public CanvasGroup vignetteGroup;

    [Range(0f, 1f)]
    public float strongestVignetteAlpha = 0.95f;

    private CanvasGroup canvasGroup;
    private Coroutine stormRoutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        if (vignetteGroup != null)
            vignetteGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        stormRoutine = StartCoroutine(SandstormCycle());
    }

    private void OnDisable()
    {
        if (stormRoutine != null)
        {
            StopCoroutine(stormRoutine);
            stormRoutine = null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (vignetteGroup != null)
            vignetteGroup.alpha = 0f;
    }

    private IEnumerator SandstormCycle()
    {
        while (true)
        {
            float waitTime = Random.Range(
                minimumWait,
                maximumWait
            );

            yield return new WaitForSeconds(waitTime);

            if (GameManager.Instance == null ||
                GameManager.Instance.CurrentMile != 1 ||
                !GameManager.Instance.IsRunActive)
                continue;

            yield return FadeStorm(
                0f,
                strongestAlpha,
                0f,
                strongestVignetteAlpha,
                buildupDuration
            );

            yield return new WaitForSeconds(strongestDuration);

            yield return FadeStorm(
                strongestAlpha,
                0f,
                strongestVignetteAlpha,
                0f,
                clearingDuration
            );
        }
    }

    private IEnumerator FadeStorm(
        float startAlpha,
        float targetAlpha,
        float startVignette,
        float targetVignette,
        float duration)
    {
        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;

            if (vignetteGroup != null)
                vignetteGroup.alpha = targetVignette;

            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (GameManager.Instance == null ||
                GameManager.Instance.CurrentMile != 1 ||
                !GameManager.Instance.IsRunActive)
            {
                canvasGroup.alpha = 0f;

                if (vignetteGroup != null)
                    vignetteGroup.alpha = 0f;

                yield break;
            }

            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(
                elapsed / duration
            );

            canvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                progress
            );

            if (vignetteGroup != null)
            {
                vignetteGroup.alpha = Mathf.Lerp(
                    startVignette,
                    targetVignette,
                    progress
                );
            }

            yield return null;
        }

        canvasGroup.alpha = targetAlpha;

        if (vignetteGroup != null)
            vignetteGroup.alpha = targetVignette;
    }
}
