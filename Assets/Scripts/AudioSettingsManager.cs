
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Volume Sliders")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private const string MASTER_KEY = "MasterVolume";
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    private void Start()
    {
        // Load previously saved settings.
        float master = PlayerPrefs.GetFloat(MASTER_KEY, 1f);
        float music = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        float sfx = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        // Initialize sliders.
        masterVolumeSlider.value = master;
        musicVolumeSlider.value = music;
        sfxVolumeSlider.value = sfx;

        // Connect slider events.
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);

        // Apply saved settings.
        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    private void SetMasterVolume(float value)
    {
        SetMixerVolume("MasterVolume", value);
        PlayerPrefs.SetFloat(MASTER_KEY, value);
    }

    private void SetMusicVolume(float value)
    {
        SetMixerVolume("MusicVolume", value);
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
    }

    private void SetSFXVolume(float value)
    {
        SetMixerVolume("SFXVolume", value);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    private void SetMixerVolume(string parameter, float value)
    {
        float decibels = value <= 0.0001f
            ? -80f
            : Mathf.Log10(value) * 20f;

        audioMixer.SetFloat(parameter, decibels);
    }

    public void ResetToDefaults()
    {
        // Reset audio settings.
        masterVolumeSlider.value = 1f;
        musicVolumeSlider.value = 1f;
        sfxVolumeSlider.value = 1f;

        // Reset display settings.
        FindFirstObjectByType<DisplaySettingsManager>()
            ?.ResetDisplayToDefaults();

        FindFirstObjectByType<GameplaySettingsManager>()
    ?.ResetGameplayToDefaults();

        PlayerPrefs.Save();
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}
