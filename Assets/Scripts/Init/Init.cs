using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using GoogleMobileAds.Api;

public class Init : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnRuntimeMethodLoad()
    {
        SetInitialConfigs();
        MobileAds.Initialize(InitializationStatus => { });
        GameManager.dataManager.LoadData();
        GameManager.saveManager.LoadSave();
    }

    static void SetInitialConfigs()
    {
        // TODO: create player preference current theme, should be dropdown
        AudioMixer mixer = Resources.Load<AudioMixer>("Sound/GameAudioMixer");
        mixer.SetFloat("MasterVolume", Mathf.Log10(PlayerPrefs.GetFloat("MasterVolume", 0.75f)) * 20);
        mixer.SetFloat("GameVolume", Mathf.Log10(PlayerPrefs.GetFloat("GameVolume", 0.75f)) * 20);
        mixer.SetFloat("MusicVolume", Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 0.75f)) * 20);
    }
}
