using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuGameplaySettings : MonoBehaviour
{
    private GameObject invertYToggleGameObject = null;
    private GameObject subtitleToggleGameObject = null;
    private GameObject firstPersonLookSensitivitySlider = null;
    private GameObject thirdPersonLookSensitivitySlider = null;

    private Image thirdPersonLookSensitivityFillImage = null;
    private Image firstPersonLookSensitivityFillImage = null;
    private Toggle invertYToggle = null;
    private Toggle subtitleToggle = null;
    private Text thirdPersonLookSensitivityAmountText = null;
    private Text firstPersonLookSensitivityAmountText = null;

    public static PauseMenuGameplaySettings instance;
    [HideInInspector]
    public CanvasGroup gameplayCanvasGroup = null;
    [HideInInspector]
    public GameObject firstSelectedButtonInGameplay = null;

    private PauseMenuSettings settingsMenuInstance = null;

    private float slidingDelay = 0.0f;
    private float maxSlidingDelay = 0.1f;

    private void Awake()
    {
        instance = this;
        gameplayCanvasGroup = GetComponent<CanvasGroup>();
        gameplayCanvasGroup.alpha = 0.0f;
        transform.gameObject.SetActive(false);

        InitTogglesAndSliderGameObjects();
        InitSlidersFill();
        InitToggles();
        InitSlidersAmountText();
        firstSelectedButtonInGameplay = invertYToggleGameObject;
    }

    private void Start()
    {
        settingsMenuInstance = PauseMenuSettings.instance;
        SetSettingsValues();
        slidingDelay = maxSlidingDelay;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            settingsMenuInstance.StartSwitchingFromGameplayCoroutine();
            return;
        }

        UpdateSlidersCheck();
        UpdateSettingsValues();
        UpdateSlidersAmountText();
    }

    private void UpdateSlidersCheck()
    {
        SliderButtonCheck(thirdPersonLookSensitivitySlider, thirdPersonLookSensitivityFillImage);
        SliderButtonCheck(firstPersonLookSensitivitySlider, firstPersonLookSensitivityFillImage);
    }

    private void UpdateSlidersAmountText()
    {
        float thirdPersonSensitivityAmount = Mathf.RoundToInt(thirdPersonLookSensitivityFillImage.fillAmount * 300.0f);
        thirdPersonLookSensitivityAmountText.text = thirdPersonSensitivityAmount.ToString();
        float firstPersonSensitivityAmount = Mathf.RoundToInt(firstPersonLookSensitivityFillImage.fillAmount * 300.0f);
        firstPersonLookSensitivityAmountText.text = firstPersonSensitivityAmount.ToString();
    }

    void InitTogglesAndSliderGameObjects()
    {
        invertYToggleGameObject = transform.GetChild(0).gameObject;
        subtitleToggleGameObject = transform.GetChild(1).gameObject;
        thirdPersonLookSensitivitySlider = transform.GetChild(2).gameObject;
        firstPersonLookSensitivitySlider = transform.GetChild(3).gameObject;
    }

    private void InitSlidersFill()
    {
        thirdPersonLookSensitivityFillImage = thirdPersonLookSensitivitySlider.transform.GetChild(thirdPersonLookSensitivitySlider.transform.childCount - 1).GetComponent<Image>();
        firstPersonLookSensitivityFillImage = firstPersonLookSensitivitySlider.transform.GetChild(firstPersonLookSensitivitySlider.transform.childCount - 1).GetComponent<Image>();
    }

    private void InitSlidersAmountText()
    {
        thirdPersonLookSensitivityAmountText = thirdPersonLookSensitivitySlider.transform.GetChild(2).GetComponent<Text>();
        firstPersonLookSensitivityAmountText = firstPersonLookSensitivitySlider.transform.GetChild(2).GetComponent<Text>();
    }

    private void InitToggles()
    {
        invertYToggle = invertYToggleGameObject.transform.GetChild(0).GetComponent<Toggle>();
        subtitleToggle = subtitleToggleGameObject.transform.GetChild(0).GetComponent<Toggle>();
    }

    private void SetSettingsValues()
    {
        thirdPersonLookSensitivityFillImage.fillAmount = settingsMenuInstance.settings.thirdPersonLookSensitivity / 300.0f;
        firstPersonLookSensitivityFillImage.fillAmount = settingsMenuInstance.settings.firstPersonLookSensitivity / 300.0f;
        invertYToggle.isOn = settingsMenuInstance.settings.invertY;
        subtitleToggle.isOn = settingsMenuInstance.settings.subtitle;
    }

    private void UpdateSettingsValues()
    {
        settingsMenuInstance.settings.thirdPersonLookSensitivity = thirdPersonLookSensitivityFillImage.fillAmount * 300.0f;
        settingsMenuInstance.settings.firstPersonLookSensitivity = firstPersonLookSensitivityFillImage.fillAmount * 300.0f;
        settingsMenuInstance.settings.invertY = invertYToggle.isOn;
        settingsMenuInstance.settings.subtitle = subtitleToggle.isOn;
    }

    public void ToggleButton(Toggle toggle)
    {
        toggle.isOn = !toggle.isOn;
    }

    public void SliderButtonCheck(GameObject slider, Image imageFill)
    {
        if (settingsMenuInstance.eventSystem.currentSelectedGameObject == slider)
        {
            float input = Input.GetAxisRaw("Horizontal");
            if (input >= 0.6f && slidingDelay >= maxSlidingDelay)
            {
                imageFill.fillAmount += 0.1f;
            }
            else if (input <= -0.6f && slidingDelay >= maxSlidingDelay)
            {
                imageFill.fillAmount -= 0.1f;
            }

            if (slidingDelay >= maxSlidingDelay)
            {
                slidingDelay = 0.0f;
            }
            else
            {
                slidingDelay += Time.deltaTime;
            }

            if (input == 0.0f)
            {
                slidingDelay = maxSlidingDelay;
            }
        }
    }
}
