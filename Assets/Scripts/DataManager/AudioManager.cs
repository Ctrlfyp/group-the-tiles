using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioSource gameSource;
    public AudioSource musicSource;
    public AudioMixer audioMixer;

    void Start()
    {
        // init volumes
        string[] volumeNames = { "MasterVolume", "GameVolume", "MusicVolume" };
        foreach (string volumeName in volumeNames)
        {
            float playerVolume = PlayerPrefs.GetFloat(volumeName, volumeName == "MusicVolume" ? 0.1f : 0.75f);
            float volume = playerVolume <= 0 ? -144f : Mathf.Log10(playerVolume) * 20;
            audioMixer.SetFloat(volumeName, volume);
        }
    }

    public void PlayOneShotSound(AudioClip clip, bool loop = false)
    {
        gameSource.loop = false;
        gameSource.PlayOneShot(clip);

    }

    public void PlaySound(AudioClip clip, bool loop = false)
    {
        gameSource.loop = loop;
        gameSource.clip = clip;
        gameSource.Play();
    }

    public void StopSound()
    {
        gameSource.loop = false;
        gameSource.clip = null;
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }
}
