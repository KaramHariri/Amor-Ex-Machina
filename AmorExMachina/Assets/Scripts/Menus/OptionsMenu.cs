using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum ToggleType
{
    INVERTY,
    SUBTITLE
}

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    MainMenu mainMenu;
    [SerializeField]
    EventSystem eventSystem;
    [SerializeField]
    CanvasGroup audioCanvasGroup = null;
    [SerializeField]
    CanvasGroup gameplayCanvasGroup = null;
    [SerializeField]
    CanvasGroup buttonsCanvasGroup = null;

    public GameObject firstSelectedButtonInOptions = null;
    public GameObject firstSelectedButtonInAudio = null;
    public GameObject firstSelectedButtonInGameplay = null;

    #region Sliders GameObjects
    [SerializeField]
    private GameObject thirdPersonLookSensitivitySlider = null;
    [SerializeField]
    private GameObject firstPersonLookSensitivitySlider = null;
    [SerializeField]
    private GameObject effectAudioSlider = null;
    [SerializeField]
    private GameObject footstepsAudioSlider = null;
    [SerializeField]
    private GameObject voiceAudioSlider = null;
    [SerializeField]
    private GameObject musicAudioSlider = null;
    #endregion

    #region Slider Fill
    private Image thirdPersonLookSensitivityFillImage = null;
    private Image firstPersonLookSensitivityFillImage = null;
    private Image effectAudioFillImage = null;
    private Image footstepsFillImage = null;
    private Image voiceAudioFillImage = null;
    private Image musicAudioFillImage = null;
    #endregion

    #region Toggles GameObjects
    [SerializeField]
    private GameObject invertYToggleGameObject = null;
    [SerializeField]
    private GameObject subtitleToggleGameObject = null;
    #endregion

    #region Toggles
    private Toggle invertYToggle = null;
    private Toggle subtitleToggle = null;
    #endregion


    GameObject currentSelectedButton = null;

    [SerializeField]
    private float fadingSpeed = 4.0f;
    [SerializeField]
    private bool inAudioMenu = false;
    [SerializeField]
    private bool inGameplayMenu = false;
    [SerializeField]
    private Settings settings = null;

    private void Start()
    {
        transform.gameObject.SetActive(false);
        audioCanvasGroup.alpha = 0.0f;
        gameplayCanvasGroup.alpha = 0.0f;

        InitSlidersFill();
        InitToggles();
        SetSettingsValues();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (inAudioMenu)
            {
                StartSwitchingFromAudioCoroutine();
                return;
            }
            else if (inGameplayMenu)
            {
                StartSwitchingFromGameplayCoroutine();
                return;
            }
            else
            {
                ExitOptionsMenu();
            }
        }

        UpdateSlidersCheck();
        UpdateSettingsValues();
    }

    void UpdateSlidersCheck()
    {
        SliderButtonCheck(thirdPersonLookSensitivitySlider, thirdPersonLookSensitivityFillImage);
        SliderButtonCheck(firstPersonLookSensitivitySlider, firstPersonLookSensitivityFillImage);
        SliderButtonCheck(effectAudioSlider, effectAudioFillImage);
        SliderButtonCheck(footstepsAudioSlider, footstepsFillImage);
        SliderButtonCheck(voiceAudioSlider, voiceAudioFillImage);
        SliderButtonCheck(musicAudioSlider, musicAudioFillImage);
    }

    public void ExitOptionsMenu()
    {
        StartCoroutine("SwitchToMainMenu");
    }

    public void StartSetSelectedButtonEnumerator()
    {
        StartCoroutine("SetSelectedButton");
    }

    IEnumerator SetSelectedButton()
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(currentSelectedButton);
    }

    IEnumerator SwitchToMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        yield return null;
        transform.gameObject.SetActive(false);
        mainMenu.StartSetSelectedButtonEnumerator();
    }

    public void StartSwitchingFromGameplayCoroutine()
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(gameplayCanvasGroup), FadeInCanvasGroup(buttonsCanvasGroup)));
        inGameplayMenu = false;
    }

    public void StartSwitchingFromAudioCoroutine()
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(audioCanvasGroup), FadeInCanvasGroup(buttonsCanvasGroup)));
        inAudioMenu = false;
    }

    public void StartSwitchingToAudioMenuCoroutine()
    {
        audioCanvasGroup.gameObject.SetActive(true);
        gameplayCanvasGroup.gameObject.SetActive(false);
        StartCoroutine(UpdateCurrentSelectedObject(firstSelectedButtonInAudio));
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(buttonsCanvasGroup), FadeInCanvasGroup(audioCanvasGroup)));
        inAudioMenu = true;
    }

    public void StartSwitchingToGameplayCoroutine()
    {
        gameplayCanvasGroup.gameObject.SetActive(true);
        audioCanvasGroup.gameObject.SetActive(false);
        StartCoroutine(UpdateCurrentSelectedObject(firstSelectedButtonInGameplay));
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(buttonsCanvasGroup), FadeInCanvasGroup(gameplayCanvasGroup)));
        inGameplayMenu = true;
    }

    IEnumerator SwitchOptionMenu(IEnumerator fadeOutEnumerator, IEnumerator fadeInEnumerator)
    {
        StartCoroutine(fadeOutEnumerator);
        yield return null;
        StartCoroutine(fadeInEnumerator);
    }

    IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroupToFade)
    {
        while(canvasGroupToFade.alpha > 0.0f)
        {
            canvasGroupToFade.alpha -= Time.deltaTime * fadingSpeed;
            yield return null;
        }
    }

    IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroupToFade)
    {
        while (canvasGroupToFade.alpha < 1.0f)
        {
            canvasGroupToFade.alpha += Time.deltaTime * fadingSpeed;
            yield return null;
        }
    }

    IEnumerator UpdateCurrentSelectedObject(GameObject nextSelectedObject)
    {
        currentSelectedButton = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(nextSelectedObject);
    }

    public void ToggleButton(Toggle toggle)
    {
        toggle.isOn = !toggle.isOn;
    }

    public void SliderButtonCheck(GameObject slider, Image imageFill)
    {
        if(eventSystem.currentSelectedGameObject == slider)
        {
            float input = Input.GetAxis("Horizontal");
            if(input >= 0.6f)
            {
                imageFill.fillAmount += 0.01f;
            }
            else if(input <= -0.6f)
            {
                imageFill.fillAmount -= 0.01f;
            }
        }
    }

    void InitSlidersFill()
    {
        thirdPersonLookSensitivityFillImage = thirdPersonLookSensitivitySlider.transform.GetChild(thirdPersonLookSensitivitySlider.transform.childCount - 1).GetComponent<Image>();
        firstPersonLookSensitivityFillImage = firstPersonLookSensitivitySlider.transform.GetChild(firstPersonLookSensitivitySlider.transform.childCount - 1).GetComponent<Image>();
        effectAudioFillImage = effectAudioSlider.transform.GetChild(effectAudioSlider.transform.childCount - 1).GetComponent<Image>();
        footstepsFillImage = footstepsAudioSlider.transform.GetChild(footstepsAudioSlider.transform.childCount - 1).GetComponent<Image>();
        voiceAudioFillImage = voiceAudioSlider.transform.GetChild(voiceAudioSlider.transform.childCount - 1).GetComponent<Image>();
        musicAudioFillImage = musicAudioSlider.transform.GetChild(musicAudioSlider.transform.childCount - 1).GetComponent<Image>();
    }

    void InitToggles()
    {
        invertYToggle = invertYToggleGameObject.transform.GetChild(0).GetComponent<Toggle>();
        subtitleToggle = subtitleToggleGameObject.transform.GetChild(0).GetComponent<Toggle>();
    }

    void SetSettingsValues()
    {
        thirdPersonLookSensitivityFillImage.fillAmount = settings.thirdPersonLookSensitivity / 300.0f;
        firstPersonLookSensitivityFillImage.fillAmount = settings.firstPersonLookSensitivity / 300.0f;
        effectAudioFillImage.fillAmount = settings.effectsVolume;
        footstepsFillImage.fillAmount = settings.footstepsVolume;
        voiceAudioFillImage.fillAmount = settings.voiceVolume;
        musicAudioFillImage.fillAmount = settings.musicVolume;
        invertYToggle.isOn = settings.invertY;
        subtitleToggle.isOn = settings.subtitle;
    }

    void UpdateSettingsValues()
    {
        settings.thirdPersonLookSensitivity = thirdPersonLookSensitivityFillImage.fillAmount * 300.0f;
        settings.firstPersonLookSensitivity = firstPersonLookSensitivityFillImage.fillAmount * 300.0f;
        settings.effectsVolume = effectAudioFillImage.fillAmount;
        settings.footstepsVolume = footstepsFillImage.fillAmount;
        settings.voiceVolume = voiceAudioFillImage.fillAmount;
        settings.musicVolume = musicAudioFillImage.fillAmount;
        settings.invertY = invertYToggle.isOn;
        settings.subtitle = subtitleToggle.isOn;
    }

}
