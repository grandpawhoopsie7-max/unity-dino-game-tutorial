
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

    [Header("Gameplay Settings")]
    public bool environmentalEffectsEnabled = true;

    private CanvasGroup canvasGroup;
    private Coroutine stormRoutine;

    private void Awake()
    {
        environmentalEffectsEnabled =
    PlayerPrefs.GetInt("EnvironmentalEffectsEnabled", 1) == 1;
        canvasGroup = GetComponent<CanvasGroup>();
        SetEffectsVisible(false);
    }

    private void OnEnable()
    {
        if (environmentalEffectsEnabled)
            stormRoutine = StartCoroutine(SandstormCycle());
    }

    private void OnDisable()
    {
        if (stormRoutine != null)
        {
            StopCoroutine(stormRoutine);
            stormRoutine = null;
        }

        SetEffectsVisible(false);
    }

    public void SetEnvironmentalEffects(bool enabled)
    {
        environmentalEffectsEnabled = enabled;

        if (!enabled)
        {
            if (stormRoutine != null)
            {
                StopCoroutine(stormRoutine);
                stormRoutine = null;
            }

            SetEffectsVisible(false);
        }
        else if (isActiveAndEnabled && stormRoutine == null)
        {
            stormRoutine = StartCoroutine(SandstormCycle());
        }
    }

    private void SetEffectsVisible(bool visible)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = visible ? canvasGroup.alpha : 0f;

        if (vignetteGroup != null)
            vignetteGroup.alpha = 0f;
    }

    private IEnumerator SandstormCycle()
    {
        while (environmentalEffectsEnabled)
        {
            float waitTime = Random.Range(
                minimumWait,
                maximumWait
            );

            yield return new WaitForSeconds(waitTime);

            if (!environmentalEffectsEnabled)
                yield break;

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

            if (!environmentalEffectsEnabled)
                yield break;

            yield return new WaitForSeconds(strongestDuration);

            if (!environmentalEffectsEnabled)
                yield break;

            yield return FadeStorm(
                strongestAlpha,
                0f,
                strongestVignetteAlpha,
                0f,
                clearingDuration
            );
        }

        stormRoutine = null;
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
            if (!environmentalEffectsEnabled ||
                GameManager.Instance == null ||
                GameManager.Instance.CurrentMile != 1 ||
                !GameManager.Instance.IsRunActive)
            {
                SetEffectsVisible(false);
                yield break;
            }

            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

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
