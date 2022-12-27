using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoard : MonoBehaviour
{
    public AudioClip OnHover;
    public AudioClip OnClick;
    public AudioClip SliderTick;
    public AudioClip Draw;
    public AudioClip MenuAppear;
    public AudioClip MenuDisappear;
    public AudioClip Purchase;
    public AudioClip Win;
    public AudioClip WinCounter;
    public AudioClip Lose;
    public AudioClip Spawn;
    public AudioClip Divide;
    public AudioClip SubmitSingle;
    public AudioClip SubmitMulti;
    public AudioClip SubmitLine;
    public AudioClip SubmitBomb;
    public AudioClip Timer;
    public AudioClip TimeOut;
    public AudioClip Bomb;
    public AudioClip Combo1;
    public AudioClip Combo2;
    public AudioClip Combo3;
    public AudioClip Combo4;
    public AudioClip Combo5;
    public AudioClip Line;
    public AudioClip Select1;
    public AudioClip Select2;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = ComponentUtility.audioManager;
    }

    public void PlayOneShotSound(AudioClip clip)
    {
        if (audioManager)
        {
            audioManager.PlayOneShotSound(clip);
        }
        else
        {
            audioManager = ComponentUtility.audioManager;
            audioManager.PlayOneShotSound(clip);
        }
    }

    public void PlaySound(AudioClip clip, bool loop = false)
    {
        if (audioManager)
        {
            audioManager.PlaySound(clip, loop);
        }
        else
        {
            audioManager = ComponentUtility.audioManager;
            audioManager.PlaySound(clip, loop);
        }
    }

    public void StopSound()
    {
        if (audioManager)
        {
            audioManager.StopSound();
        }
        else
        {
            audioManager = ComponentUtility.audioManager;
            audioManager.StopSound();
        }
    }
}
