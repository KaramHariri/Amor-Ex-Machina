using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    private GameObject effectAudioSlider = null;
    private GameObject footstepsAudioSlider = null;
    private GameObject voiceAudioSlider = null;
    private GameObject musicAudioSlider = null;

    private Image effectAudioFillImage = null;
    private Image footstepsFillImage = null;
    private Image voiceAudioFillImage = null;
    private Image musicAudioFillImage = null;

    public static AudioSettingsMenu instance = null;
    [HideInInspector]
    public CanvasGroup audioCanvasGroup = null;
    [HideInInspector]
    public GameObject firstSelectedButtonInAudio = null;

    private OptionsMenu optionsMenuInstance = null;

    private float slidingDelay = 0.0f;
    private float maxSlidingDelay = 0.1f;
    private void Awake()
    {
        instance = this;
        audioCanvasGroup = GetComponent<CanvasGroup>();
        audioCanvasGroup.alpha = 0.0f;
        transform.gameObject.SetActive(false);

        InitSliderGameObjects();
        InitSlidersFill();
       
        firstSelectedButtonInAudio = effectAudioSlider;
    }

    private void Start()
    {
        optionsMenuInstance = OptionsMenu.instance;
        SetSettingsValues();
        slidingDelay = maxSlidingDelay;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            optionsMenuInstance.StartSwitchingFromAudioCoroutine();
            return;
        }

        UpdateSlidersCheck();
        UpdateSettingsValues();
    }

    private void UpdateSlidersCheck()
    {
        SliderButtonCheck(effectAudioSlider, effectAudioFillImage);
        SliderButtonCheck(footstepsAudioSlider, footstepsFillImage);
        SliderButtonCheck(voiceAudioSlider, voiceAudioFillImage);
        SliderButtonCheck(musicAudioSlider, musicAudioFillImage);
    }

    private void InitSliderGameObjects()
    {
        effectAudioSlider = transform.GetChild(0).gameObject;
        footstepsAudioSlider = transform.GetChild(1).gameObject;
        voiceAudioSlider = transform.GetChild(2).gameObject;
        musicAudioSlider = transform.GetChild(3).gameObject;
    }

    private void InitSlidersFill()
    {
        effectAudioFillImage = effectAudioSlider.transform.GetChild(effectAudioSlider.transform.childCount - 1).GetComponent<Image>();
        footstepsFillImage = footstepsAudioSlider.transform.GetChild(footstepsAudioSlider.transform.childCount - 1).GetComponent<Image>();
        voiceAudioFillImage = voiceAudioSlider.transform.GetChild(voiceAudioSlider.transform.childCount - 1).GetComponent<Image>();
        musicAudioFillImage = musicAudioSlider.transform.GetChild(musicAudioSlider.transform.childCount - 1).GetComponent<Image>();
    }

    private void SetSettingsValues()
    {
        effectAudioFillImage.fillAmount = optionsMenuInstance.settings.effectsVolume;
        footstepsFillImage.fillAmount = optionsMenuInstance.settings.footstepsVolume;
        voiceAudioFillImage.fillAmount = optionsMenuInstance.settings.voiceVolume;
        musicAudioFillImage.fillAmount = optionsMenuInstance.settings.musicVolume;
    }

    private void UpdateSettingsValues()
    {
        optionsMenuInstance.settings.effectsVolume = effectAudioFillImage.fillAmount;
        optionsMenuInstance.settings.footstepsVolume = footstepsFillImage.fillAmount;
        optionsMenuInstance.settings.voiceVolume = voiceAudioFillImage.fillAmount;
        optionsMenuInstance.settings.musicVolume = musicAudioFillImage.fillAmount;
    }

    public void SliderButtonCheck(GameObject slider, Image imageFill)
    {
        if (optionsMenuInstance.eventSystem.currentSelectedGameObject == slider)
        {
            float input = Input.GetAxis("Horizontal");
            if (input >= 0.6f && slidingDelay >= maxSlidingDelay)
            {
                imageFill.fillAmount += 0.01f;
            }
            else if (input <= -0.6f && slidingDelay >= maxSlidingDelay)
            {
                imageFill.fillAmount -= 0.01f;
            }

            if (slidingDelay >= maxSlidingDelay)
            {
                slidingDelay = 0.0f;
            }
            else
            {
                slidingDelay += Time.deltaTime;
            }
        }
    }
}
