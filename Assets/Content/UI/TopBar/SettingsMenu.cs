using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    
    public static event UnityAction<int> onThemeChange;

    public UI.DesignSystem.UISlider masterVolumeSlider;
    public UI.DesignSystem.UISlider gameVolumeSlider;
    public UI.DesignSystem.UISlider musicVolumeSlider;
    public UI.DesignSystem.UIDropdown languageDropdown;
    public UI.DesignSystem.UIDropdown themeDropdown;
    public UI.DesignSystem.UIButton quitButton;


    // TODO: Change to only show unlocked themes
    public List<string> allThemesList;

    void Start()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.75f) * 100;
        gameVolumeSlider.value = PlayerPrefs.GetFloat("GameVolume", 0.75f) * 100;
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.1f) * 100;

        masterVolumeSlider.OnValueChanged += OnMasterVolumeValueChanged;
        gameVolumeSlider.OnValueChanged += OnGameVolumeValueChanged;
        musicVolumeSlider.OnValueChanged += OnMusicVolumeValueChanged;

        languageDropdown.value = PlayerPrefs.GetInt(nameof(languageDropdown), 50);
        languageDropdown.OnValueChanged += OnLanguageChanged;


        UI.PersistentTopBar persistentTopBar = ComponentUtility.topBar;
        if (persistentTopBar && !persistentTopBar.isGameMode)
        {
            quitButton.gameObject.SetActive(false);
        }

    }

    void OnEnable()
    {
        // this has to be called every time so it's updated
        InitThemeDropdown();
    }

    private void InitThemeDropdown()
    {
        // generate all theme list
        allThemesList = GameManager.dataManager.GetAllThemeNames();

        // select previously selected theme
        string selectedThemePrefs = PlayerPrefs.GetString(Constants.playerPrefThemeKey, "DefaultTheme");
        int selectedThemeIndex = allThemesList.IndexOf(selectedThemePrefs);

        themeDropdown.SetOptions(allThemesList, selectedThemeIndex);

        // subscribe to OnDropdownChange
        themeDropdown.OnValueChanged += OnThemeChanged;
    }

    public void OnMasterVolumeValueChanged(float sliderValue)
    {
        float sliderVolume = sliderValue / 100;
        float volume = sliderVolume <= 0 ? -144f : Mathf.Log10(sliderVolume) * 20;
        audioMixer.SetFloat("MasterVolume", volume);
        PlayerPrefs.SetFloat("MasterVolume", sliderVolume);
        PlayerPrefs.Save();
    }

    public void OnGameVolumeValueChanged(float sliderValue)
    {
        float sliderVolume = sliderValue / 100;
        float volume = sliderVolume <= 0 ? -144f : Mathf.Log10(sliderVolume) * 20;
        audioMixer.SetFloat("GameVolume", volume);
        PlayerPrefs.SetFloat("GameVolume", sliderVolume);
        PlayerPrefs.Save();
    }

    public void OnMusicVolumeValueChanged(float sliderValue)
    {
        float sliderVolume = sliderValue / 100;
        float volume = sliderVolume <= 0 ? -144f : Mathf.Log10(sliderVolume) * 20;
        audioMixer.SetFloat("MusicVolume", volume);
        PlayerPrefs.SetFloat("MusicVolume", sliderVolume);
        PlayerPrefs.Save();
    }

    public void OnLanguageChanged(int dropdown)
    {
        PlayerPrefs.Save();
    }

    public void OnThemeChanged(int themeIndex)
    {
        string selectedTheme = allThemesList[themeIndex];
        GameManager.dataManager.LoadThemeFromName(selectedTheme, true);
        PlayerPrefs.SetString(Constants.playerPrefThemeKey, selectedTheme);
        PlayerPrefs.Save();
        onThemeChange?.Invoke(themeIndex);
    }

    public void OnResetButtonClicked()
    {
        GameManager.saveManager.ResetSave();
    }

    public void OnQuitButtonClicked()
    {
        ComponentUtility.LoadScene("Assets/Content/UI/MainMenuScene/MainMenuScene.unity");
    }
}
