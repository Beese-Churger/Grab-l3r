using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] public Slider SFXvolumeSlider;
    [SerializeField] public Slider BGMvolumeSlider;

    public SoundScript[] bgmSounds, sfxSounds, bgSounds;
    public AudioSource bgmSource, sfxSource, bgSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SFXvolumeSlider.value = sfxSource.volume;
            BGMvolumeSlider.value = bgmSource.volume;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(string name)
    {
        SoundScript s = Array.Find(bgmSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            bgmSource.clip = s.clip;
            bgmSource.PlayOneShot(s.clip);
        }
    }

    public void PlayBGMLoop(string name, bool stop)
    {
        SoundScript s = Array.Find(bgmSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            bgmSource.clip = s.clip;
            if (stop)
            {
                bgmSource.loop = false;
                bgmSource.Stop();
            }
            else
            {
                bgmSource.loop = true;
            }
            bgmSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        SoundScript s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void PlaySFXLoop(string name, bool stop)
    {
        SoundScript s = Array.Find(bgmSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            sfxSource.clip = s.clip;
            if (stop)
            {
                sfxSource.loop = false;
                sfxSource.Stop();
            }
            else
            {
                sfxSource.loop = true;
            }
            sfxSource.Play();
        }
    }

    public void PlayBG(string name)
    {
        SoundScript s = Array.Find(bgSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            bgSource.clip = s.clip;
            bgSource.PlayOneShot(s.clip);
        }
    }

    public void PlayBGLoop(string name, bool stop)
    {
        SoundScript s = Array.Find(bgSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            bgSource.clip = s.clip;
            if (stop)
            {
                bgSource.loop = false;
                bgSource.Stop();
            }
            else
            {
                bgSource.loop = true;
            }
            bgSource.Play();
        }
    }
  
    public void ToggleBGM()
    {
        bgmSource.mute = !bgmSource.mute;
        bgSource.mute = !bgSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void BGMVolume(float volume)
    {
        bgmSource.volume = volume;
        //bgSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}