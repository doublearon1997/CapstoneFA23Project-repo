using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] private bool canUse = false;
    [SerializeField] private MenuController menuController;

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text BGMVolumeTextValue = null;
    [SerializeField] private TMP_Text SEVolumeTextValue = null;
    [SerializeField] private Slider BGMVolumeSlider = null;
    [SerializeField] private Slider SEVolumeSlider = null;
    [SerializeField] private AudioMixer mixer = null;

    [Header("Brightness Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private UnityEngine.UI.Image brightnessPane = null;

    [Header("Fullscreen Settings")]
    [SerializeField] private Toggle fullscreenToggle;

    private void Awake()
    {
        if (canUse)
        {
            if (PlayerPrefs.HasKey("BGMVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("BGMVolume");

                BGMVolumeTextValue.text = localVolume.ToString("0.00");
                BGMVolumeSlider.value = localVolume;

                mixer.SetFloat("BGM", Mathf.Log10(localVolume) * 20);
            }
            else
                menuController.ResetButton("Audio");

            if (PlayerPrefs.HasKey("SEVolume"))
            {
                float localVolume = PlayerPrefs.GetFloat("SEVolume");

                SEVolumeTextValue.text = localVolume.ToString("0.00");
                SEVolumeSlider.value = localVolume;

                mixer.SetFloat("SE", Mathf.Log10(localVolume) * 20);
            }
            else
                menuController.ResetButton("Audio");

            if (PlayerPrefs.HasKey("masterFullscreen"))
            {
                int localFullscreen = PlayerPrefs.GetInt("masterFullscreen");

                if (localFullscreen == 1)
                {
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    fullscreenToggle.isOn = true;
                    
                }
                else
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    fullscreenToggle.isOn = false;
                }

                
            }

            if (PlayerPrefs.HasKey("masterBrightness"))
            {
                float localBrightness = PlayerPrefs.GetFloat("masterBrightness");

                brightnessTextValue.text = localBrightness.ToString("0.0");
                brightnessSlider.value = localBrightness;

                Color newBrightness = brightnessPane.color;

                if (1 - localBrightness > 0.95f)
                    newBrightness.a = 0.95f;
                else
                    newBrightness.a = 1 - localBrightness;

                brightnessPane.color = newBrightness;

            }
        }
    }

}
