using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Gameplay")]
    public Toggle cameraShakeToggle;
    public Toggle damageFlashToggle;

    void Start()
    {
        LoadSettings();
    }

    public void ApplySettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterSlider.value);
        AudioListener.volume = masterSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
        PlayerPrefs.SetInt("CameraShake", cameraShakeToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("DamageFlash", damageFlashToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        masterSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        masterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        cameraShakeToggle.isOn = PlayerPrefs.GetInt("CameraShake", 1) == 1;
        damageFlashToggle.isOn = PlayerPrefs.GetInt("DamageFlash", 1) == 1;
    }
}
