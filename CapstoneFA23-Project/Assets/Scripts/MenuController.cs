using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Globalization;
using TMPro;


public class MenuController : MonoBehaviour
{
    int currentSettingsSubMenu;

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text BGMVolumeTextValue = null;
    [SerializeField] private TMP_Text SEVolumeTextValue = null;
    [SerializeField] private Slider BGMVolumeSlider = null;
    [SerializeField] private Slider SEVolumeSlider = null;
    [SerializeField] private float defaultVolume = 0.9f;
    [SerializeField] private AudioMixer mixer = null;

    [Header("Graphics Settings")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private float defaultBrightness = 1;
    [SerializeField] private UnityEngine.UI.Image brightnessPane = null;

    private bool isFullscreen;
    private float brightnessLevel;

    [SerializeField] private Toggle fullscreenToggle;

    [Header("Resolution Dropdown")]
    public TMP_Dropdown resolutionDropdown;
    private List<Resolution> resolutions;

    [Header("Instance Managers")]
    public SEManager seManager;
    public BGMManager bgmManager;
    public PartyController myPartyController;
    public InventoryController myInventoryController;
    public GameObject MenuObject, SubMenuObject, PartyInvMenuObject, SettingsMenuObject;

    public void Start()
    {
        Resolution[] unprunedResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        resolutions = new List<Resolution>();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;
        int nonRepeatedIndex = 0;

        for(int i = 0; i < unprunedResolutions.Length; i++) 
        { 
            string option = unprunedResolutions[i].width + " x " + unprunedResolutions[i].height;

            if(!options.Contains(option))
            {
                resolutions.Add(unprunedResolutions[i]);
                nonRepeatedIndex++;
                
                options.Add(option);

                if (unprunedResolutions[i].width == Screen.width && unprunedResolutions[i].height == Screen.height)
                    currentResolutionIndex = nonRepeatedIndex;
            }
           
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        if(fullscreenToggle.isOn)
            SetFullscreen(true);
        else
            SetFullscreen(false);

        SetBrightness(brightnessSlider.value);

        BGMManager.instance.PlayBGM("atTheEnd");
    }

    public void Update()
    {
        //if escape is pressed close open UI panels
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SubMenuObject.activeSelf || PartyInvMenuObject.activeSelf || SettingsMenuObject.activeSelf)
            {
                SubMenuObject.SetActive(false);
                PartyInvMenuObject.SetActive(false);
                SettingsMenuObject.SetActive(false);
                MenuObject.SetActive(true);
            }
            else
            {
                SubMenuObject.gameObject.SetActive(!SubMenuObject.gameObject.activeSelf);
                if (SubMenuObject.activeSelf)
                {
                    myInventoryController.updateInventory();
                    myPartyController.updatePartyUI();
                }
            }
        }
    }

    public void SetResolution(int resolutionIndex, FullScreenMode fullScreenMode)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, fullScreenMode);
    }

    public void BeginGame()
    {
        //SceneManager.LoadScene(something)
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetBGMVolume(float volume)
    {
        mixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        BGMVolumeTextValue.text = volume.ToString("0.00");
    }

    public void SetSEVolume(float volume)
    {
        mixer.SetFloat("SE", Mathf.Log10(volume) * 20);
        SEVolumeTextValue.text = volume.ToString("0.00");
    }

    public void ExitAudioWithoutSaving()
    {
        if(BGMVolumeSlider.value != PlayerPrefs.GetFloat("BGMVolume"))
        {
            mixer.SetFloat("BGM", Mathf.Log10(PlayerPrefs.GetFloat("BGMVolume")) * 20);
            BGMVolumeSlider.value = PlayerPrefs.GetFloat("BGMVolume");
            BGMVolumeTextValue.text = PlayerPrefs.GetFloat("BGMVolume").ToString("0.00");
        }

        if (SEVolumeSlider.value != PlayerPrefs.GetFloat("SEVolume"))
        {
            mixer.SetFloat("SE", Mathf.Log10(PlayerPrefs.GetFloat("SEVolume")) * 20);
            SEVolumeSlider.value = PlayerPrefs.GetFloat("SEVolume");
            SEVolumeTextValue.text = PlayerPrefs.GetFloat("SEVolume").ToString("0.00");
        }
    }

    public void ExitGraphicsWithoutSaving()
    {
        if(brightnessSlider.value != PlayerPrefs.GetFloat("masterBrightness")){
            brightnessLevel = PlayerPrefs.GetFloat("masterBrightness");
            brightnessSlider.value = PlayerPrefs.GetFloat("masterBrightness");
            brightnessTextValue.text = PlayerPrefs.GetFloat("masterBrightness").ToString();
        }
    }

    public void ExitGamePlayWithoutSaving()
    {
        //do stuff
    }

    public void ResetButton(string MenuType)
    {
        if(MenuType == "Audio")
        {
            mixer.SetFloat("BGM", Mathf.Log10(defaultVolume) * 20);
            mixer.SetFloat("SE", Mathf.Log10(defaultVolume) * 20);

            BGMVolumeSlider.value = defaultVolume;
            SEVolumeSlider.value = defaultVolume;

            BGMVolumeTextValue.text = defaultVolume.ToString("0.00");
            SEVolumeTextValue.text = defaultVolume.ToString("0.00");

            ApplyVolume();
        }
        else if (MenuType == "Graphics")
        {
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0.0");

            fullscreenToggle.isOn = true;
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Count;

            ApplyGraphicsSettings();
        }
        else if (MenuType == "Gameplay")
        {
            ApplyGameplaySettings();
        }
    }

    public void ApplyVolume()
    {
        PlayerPrefs.SetFloat("BGMVolume", BGMVolumeSlider.value);
        PlayerPrefs.SetFloat("SEVolume", SEVolumeSlider.value);
    }

    public void SetBrightness(float brightness)
    {
        brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullscreen(bool isFullscreen)
    {
        this.isFullscreen = isFullscreen;
    }

    public void ApplyGraphicsSettings()
    {
        UnityEngine.Debug.Log("fullscreen: " + this.isFullscreen);
        PlayerPrefs.SetFloat("masterBrightness", brightnessLevel);
        PlayerPrefs.SetInt("masterFullscreen", (this.isFullscreen ? 1 : 0));
        UnityEngine.Debug.Log("isFullscreen: " + this.isFullscreen);

        Color newBrightness = brightnessPane.color;

        if (1 - brightnessLevel > 0.95f)
            newBrightness.a = 0.95f;
        else
            newBrightness.a = 1 - this.brightnessLevel;

        brightnessPane.color = newBrightness;
   
        FullScreenMode fullScreenMode;

        if (this.isFullscreen)
            fullScreenMode = FullScreenMode.FullScreenWindow;
        else
            fullScreenMode = FullScreenMode.Windowed;

        SetResolution(resolutionDropdown.value, fullScreenMode);

    }

    public void ApplyGameplaySettings()
    {
        //do stuff
    }
}
