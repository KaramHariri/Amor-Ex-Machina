using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySettingsMenu : MonoBehaviour
{
    private GameObject invertYToggleGameObject = null;
    private GameObject subtitleToggleGameObject = null;
    private GameObject firstPersonLookSensitivitySlider = null;
    private GameObject thirdPersonLookSensitivitySlider = null;

    private Image thirdPersonLookSensitivityFillImage = null;
    private Image firstPersonLookSensitivityFillImage = null;
    private Toggle invertYToggle = null;
    private Toggle subtitleToggle = null;

    public static GameplaySettingsMenu instance;
    [HideInInspector]
    public CanvasGroup gameplayCanvasGroup = null;
    [HideInInspector]
    public GameObject firstSelectedButtonInGameplay = null;

    private OptionsMenu optionsMenuInstance = null;

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
        firstSelectedButtonInGameplay = invertYToggleGameObject;
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
            optionsMenuInstance.StartSwitchingFromGameplayCoroutine();
            return;
        }

        UpdateSlidersCheck();
        UpdateSettingsValues();
    }

    private void UpdateSlidersCheck()
    {
        SliderButtonCheck(thirdPersonLookSensitivitySlider, thirdPersonLookSensitivityFillImage);
        SliderButtonCheck(firstPersonLookSensitivitySlider, firstPersonLookSensitivityFillImage);
    }

    void InitTogglesAndSliderGameObjects()
    {
        invertYToggleGameObject = transform.GetChild(0).gameObject;
        subtitleToggleGameObject = transform.GetChild(1).gameObject;
        firstPersonLookSensitivitySlider = transform.GetChild(2).gameObject;
        thirdPersonLookSensitivitySlider = transform.GetChild(3).gameObject;
    }

    private void InitSlidersFill()
    {
        thirdPersonLookSensitivityFillImage = thirdPersonLookSensitivitySlider.transform.GetChild(thirdPersonLookSensitivitySlider.transform.childCount - 1).GetComponent<Image>();
        firstPersonLookSensitivityFillImage = firstPersonLookSensitivitySlider.transform.GetChild(firstPersonLookSensitivitySlider.transform.childCount - 1).GetComponent<Image>();
    }

    private void InitToggles()
    {
        invertYToggle = invertYToggleGameObject.transform.GetChild(0).GetComponent<Toggle>();
        subtitleToggle = subtitleToggleGameObject.transform.GetChild(0).GetComponent<Toggle>();
    }

    private void SetSettingsValues()
    {
        thirdPersonLookSensitivityFillImage.fillAmount = optionsMenuInstance.settings.thirdPersonLookSensitivity / 300.0f;
        firstPersonLookSensitivityFillImage.fillAmount = optionsMenuInstance.settings.firstPersonLookSensitivity / 300.0f;
        invertYToggle.isOn = optionsMenuInstance.settings.invertY;
        subtitleToggle.isOn = optionsMenuInstance.settings.subtitle;
    }

    private void UpdateSettingsValues()
    {
        optionsMenuInstance.settings.thirdPersonLookSensitivity = thirdPersonLookSensitivityFillImage.fillAmount * 300.0f;
        optionsMenuInstance.settings.firstPersonLookSensitivity = firstPersonLookSensitivityFillImage.fillAmount * 300.0f;
        optionsMenuInstance.settings.invertY = invertYToggle.isOn;
        optionsMenuInstance.settings.subtitle = subtitleToggle.isOn;
    }

    public void ToggleButton(Toggle toggle)
    {
        toggle.isOn = !toggle.isOn;
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

            if(slidingDelay >= maxSlidingDelay)
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
