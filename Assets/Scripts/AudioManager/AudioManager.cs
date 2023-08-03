using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    //public Slider SFXvolumeSlider;
    //public Slider BGMvolumeSlider;

    public SoundScript[] bgmSounds, sfxSounds, bgSounds;
    public AudioSource bgmSource, sfxSource, bgSource;

    public GameObject sfx;
    public AudioMixer audioMixerGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
            //SFXvolumeSlider.value = sfxSource.volume;
            //BGMvolumeSlider.value = bgmSource.volume;
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
            if (bgmSource != null)
            {
                if (!bgmSource.isPlaying)
                {
                    bgmSource.clip = s.clip;
                    bgmSource.PlayOneShot(s.clip);
                }
            }
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

            if (bgmSource != null)
                if (!bgmSource.isPlaying)
                    bgmSource.Play();
            if (stop)
            {
                bgmSource.loop = false;
                bgmSource.Stop();
            }
            else
            {
                bgmSource.loop = true;
            }

        }
    }

    public void PlaySFX(string name, Vector3 position)
    {
        SoundScript s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.Log("Sound not found");
        }
        else
        {
            GameObject go = Instantiate(sfx, position, Quaternion.identity);
            AudioSource tempSource = go.GetComponent<AudioSource>();
            tempSource.clip = s.clip;
            tempSource.volume = (s.volume * 0.01f) * sfxSource.volume;
            go.name = name + "_AudioClip";
            go.GetComponent<PlayOnAwake>().Play();
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
        if (volume == 0f)
            audioMixerGroup.SetFloat("BGMVolume", -80f);
        else
            audioMixerGroup.SetFloat("BGMVolume", Mathf.Log10(volume) * 10f);        
    }

    public void SFXVolume(float volume)
    {
        if (volume == 0f)
            audioMixerGroup.SetFloat("SFXVolume", -80f);
        else
            audioMixerGroup.SetFloat("SFXVolume", Mathf.Log10(volume) * 10f);
    }
    public void StopBGM()
    {
        bgmSource.Stop();
    }
    public void MasterVolume(float volume)
    {
        if (volume == 0f)
            audioMixerGroup.SetFloat("MasterVolume", -80f);
        else
            audioMixerGroup.SetFloat("MasterVolume", Mathf.Log10(volume) * 5f);
    }

}