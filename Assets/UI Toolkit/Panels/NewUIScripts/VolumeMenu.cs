using System;
using UnityEngine;
using UnityEngine.UIElements;

public class VolumeMenu
{
    private Button _backButton;
    private Slider _sfxSlider;
    private Slider _bgmSlider;
    private Slider _masterSlider;



    public Action BackAction { set => _backButton.clicked += value; }

    public VolumeMenu (VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
        _sfxSlider = root.Q<Slider>("SFXVolumeSlider");
        _bgmSlider = root.Q<Slider>("MusicVolumeSlider");
        _masterSlider = root.Q<Slider>("MasterVolumeSlider");

        AudioManager.Instance.audioMixerGroup.ClearFloat("MasterSlider");

        _bgmSlider.value = AudioManager.Instance.bgmSource.volume * 100f;
        _bgmSlider.RegisterValueChangedCallback((evt) => { AudioManager.Instance.BGMVolume(evt.newValue); });

        _sfxSlider.value = AudioManager.Instance.sfxSource.volume * 100;
        _sfxSlider.RegisterValueChangedCallback((evt) => { AudioManager.Instance.SFXVolume(evt.newValue); });

        _masterSlider.value = AudioManager.Instance.bgmSource.volume * 100f;
        _masterSlider.RegisterValueChangedCallback((evt) => { AudioManager.Instance.MasterVolume(evt.newValue); });

    }

}
