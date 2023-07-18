using System;
using UnityEngine.UIElements;

public class VolumeMenu
{
    private Button _backButton;
    private Slider _slider;
    public Action BackAction { set => _backButton.clicked += value; }

    public VolumeMenu (VisualElement root)
    {
        _backButton = root.Q<Button>("BackButton");
        _slider = root.Q<Slider>("VolumeSlider");
        _slider.RegisterValueChangedCallback((evt) => { AudioManager.Instance.BGMVolume(evt.newValue * 0.01f); });
        _slider.value = AudioManager.Instance.bgmSource.volume * 100;
        //_fullscreenToggle.RegisterCallback<MouseUpEvent>((evt) => { SetFullscreen(); }, TrickleDown.TrickleDown);

    }

}
