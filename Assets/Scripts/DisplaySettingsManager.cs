
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DisplaySettingsManager : MonoBehaviour
{
    [Header("Display Settings")]
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public Button applyDisplayButton;

    private const string FULLSCREEN_KEY = "FullscreenEnabled";
    private const string WIDTH_KEY = "ScreenWidth";
    private const string HEIGHT_KEY = "ScreenHeight";
    private const string REFRESH_KEY = "ScreenRefreshRate";

    private Resolution[] availableResolutions;

    private void Start()
    {
        // Load saved fullscreen preference.
        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_KEY, 1) == 1;

        fullscreenToggle.isOn = fullscreen;
        ApplyFullscreen(fullscreen);

        fullscreenToggle.onValueChanged.AddListener(ApplyFullscreen);

        // Load available resolutions.
        availableResolutions = Screen.resolutions;

        PopulateResolutionDropdown();

        // Connect Apply button.
        applyDisplayButton.onClick.AddListener(ApplySelectedResolution);
    }

    public void ResetDisplayToDefaults()
{
    // Restore recommended display resolution.
    Resolution recommended = Screen.currentResolution;

    int recommendedIndex = 0;

    for (int i = 0; i < availableResolutions.Length; i++)
    {
        if (availableResolutions[i].width == recommended.width &&
            availableResolutions[i].height == recommended.height &&
            GetRefreshRate(availableResolutions[i]) == GetRefreshRate(recommended))
        {
            recommendedIndex = i;
            break;
        }
    }

    // Restore fullscreen.
    fullscreenToggle.isOn = true;
    ApplyFullscreen(true);

    // Select recommended resolution.
    resolutionDropdown.value = recommendedIndex;
    resolutionDropdown.RefreshShownValue();

    // Apply recommended resolution immediately.
    ApplySelectedResolution();
}
    private void PopulateResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        // Sort resolutions from highest to lowest.
        System.Array.Sort(availableResolutions, (a, b) =>
        {
            int widthComparison = b.width.CompareTo(a.width);

            if (widthComparison != 0)
                return widthComparison;

            return b.height.CompareTo(a.height);
        });

        // Find the monitor's native resolution.
        Resolution nativeResolution = Screen.currentResolution;

        int nativeWidth = nativeResolution.width;
        int nativeHeight = nativeResolution.height;
        int nativeRefresh = GetRefreshRate(nativeResolution);

        // Find the native resolution in the available list.
        int recommendedIndex = -1;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution r = availableResolutions[i];

            if (r.width == nativeWidth &&
                r.height == nativeHeight &&
                GetRefreshRate(r) == nativeRefresh)
            {
                recommendedIndex = i;
                break;
            }
        }

        // Move recommended resolution to the top.
        if (recommendedIndex > 0)
        {
            Resolution recommended = availableResolutions[recommendedIndex];

            for (int i = recommendedIndex; i > 0; i--)
                availableResolutions[i] = availableResolutions[i - 1];

            availableResolutions[0] = recommended;
        }

        // Create dropdown labels.
        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution r = availableResolutions[i];

            string label = $"{r.width} x {r.height} ({GetRefreshRate(r)} Hz)";

            if (i == 0)
                label += " - RECOMMENDED";

            options.Add(label);
        }

        resolutionDropdown.AddOptions(options);

        // Restore saved resolution if available.
        int savedWidth = PlayerPrefs.GetInt(WIDTH_KEY, nativeWidth);
        int savedHeight = PlayerPrefs.GetInt(HEIGHT_KEY, nativeHeight);
        int savedRefresh = PlayerPrefs.GetInt(REFRESH_KEY, nativeRefresh);

        int selectedIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            Resolution r = availableResolutions[i];

            if (r.width == savedWidth &&
                r.height == savedHeight &&
                GetRefreshRate(r) == savedRefresh)
            {
                selectedIndex = i;
                break;
            }
        }

        resolutionDropdown.value = selectedIndex;
        resolutionDropdown.RefreshShownValue();
    }


    public void ApplyFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt(FULLSCREEN_KEY, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ApplySelectedResolution()
    {
        int index = resolutionDropdown.value;

        if (index < 0 || index >= availableResolutions.Length)
            return;

        Resolution resolution = availableResolutions[index];

        int refreshRate = GetRefreshRate(resolution);

        Screen.SetResolution(
            resolution.width,
            resolution.height,
            Screen.fullScreenMode,
            resolution.refreshRateRatio
        );

        PlayerPrefs.SetInt(WIDTH_KEY, resolution.width);
        PlayerPrefs.SetInt(HEIGHT_KEY, resolution.height);
        PlayerPrefs.SetInt(REFRESH_KEY, refreshRate);
        PlayerPrefs.Save();

        Debug.Log(
            $"Applied resolution: {resolution.width} x {resolution.height} at {refreshRate} Hz"
        );
    }

    private int GetCurrentRefreshRate()
    {
        return GetRefreshRate(Screen.currentResolution);
    }

    private int GetRefreshRate(Resolution resolution)
    {
        return Mathf.RoundToInt(
            (float)resolution.refreshRateRatio.value
        );
    }

    private void OnDestroy()
    {
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.RemoveListener(ApplyFullscreen);

        if (applyDisplayButton != null)
            applyDisplayButton.onClick.RemoveListener(ApplySelectedResolution);
    }
}
