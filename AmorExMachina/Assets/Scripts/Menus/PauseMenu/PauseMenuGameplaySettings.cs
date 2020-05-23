using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenuGameplaySettings : MonoBehaviour
{
    private GameObject invertYToggleGameObject = null;
    private GameObject firstPersonLookSensitivitySlider = null;
    private GameObject thirdPersonLookSensitivitySlider = null;
    private GameObject backGameObject = null;

    private Image thirdPersonLookSensitivityFillImage = null;
    private Image firstPersonLookSensitivityFillImage = null;
    private Toggle invertYToggle = null;
    private TextMeshProUGUI thirdPersonLookSensitivityAmountText = null;
    private TextMeshProUGUI firstPersonLookSensitivityAmountText = null;

    [SerializeField] private TextMeshProUGUI invertYText = null;
    [SerializeField] private TextMeshProUGUI thirdPersonLookSensitivityText = null;
    [SerializeField] private TextMeshProUGUI firstPersonLookSensitivityText = null;
    [SerializeField] private TextMeshProUGUI backText = null;

    public static PauseMenuGameplaySettings instance;
    [HideInInspector]
    public CanvasGroup gameplayCanvasGroup = null;
    [HideInInspector]
    public GameObject firstSelectedButtonInGameplay = null;
    private GameObject lastSelectedButton = null;

    private PauseMenuSettings settingsMenuInstance = null;

    private float slidingDelay = 0.0f;
    private float maxSlidingDelay = 0.1f;

    private AudioManager audioManager = null;
    private EventSystem eventSystem = null;

    private void Awake()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
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
        audioManager = GameHandler.audioManager;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            settingsMenuInstance.StartSwitchingFromGameplayCoroutine();
            return;
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }

        SelectedButton();
        UpdateSlidersCheck();
        UpdateSettingsValues();
        UpdateSlidersAmountText();
    }

    void SelectedButton()
    {
        invertYText.color = Color.white;
        thirdPersonLookSensitivityText.color = Color.white;
        firstPersonLookSensitivityText.color = Color.white;
        backText.color = Color.white;

        if (eventSystem.currentSelectedGameObject == invertYToggleGameObject.gameObject)
        {
            invertYText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == thirdPersonLookSensitivitySlider.gameObject)
        {
            thirdPersonLookSensitivityText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == firstPersonLookSensitivitySlider.gameObject)
        {
            firstPersonLookSensitivityText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == backGameObject.gameObject)
        {
            backText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
        }
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
        thirdPersonLookSensitivitySlider = transform.GetChild(1).gameObject;
        firstPersonLookSensitivitySlider = transform.GetChild(2).gameObject;
        backGameObject = transform.GetChild(3).gameObject;
    }

    private void InitSlidersFill()
    {
        thirdPersonLookSensitivityFillImage = thirdPersonLookSensitivitySlider.transform.GetChild(thirdPersonLookSensitivitySlider.transform.childCount - 1).GetComponent<Image>();
        firstPersonLookSensitivityFillImage = firstPersonLookSensitivitySlider.transform.GetChild(firstPersonLookSensitivitySlider.transform.childCount - 1).GetComponent<Image>();
    }

    private void InitSlidersAmountText()
    {
        thirdPersonLookSensitivityAmountText = thirdPersonLookSensitivitySlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        firstPersonLookSensitivityAmountText = firstPersonLookSensitivitySlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    private void InitToggles()
    {
        invertYToggle = invertYToggleGameObject.transform.GetChild(0).GetComponent<Toggle>();
    }

    private void SetSettingsValues()
    {
        thirdPersonLookSensitivityFillImage.fillAmount = settingsMenuInstance.settings.thirdPersonLookSensitivity / 300.0f;
        firstPersonLookSensitivityFillImage.fillAmount = settingsMenuInstance.settings.firstPersonLookSensitivity / 300.0f;
        invertYToggle.isOn = settingsMenuInstance.settings.invertY;
    }

    private void UpdateSettingsValues()
    {
        settingsMenuInstance.settings.thirdPersonLookSensitivity = thirdPersonLookSensitivityFillImage.fillAmount * 300.0f;
        settingsMenuInstance.settings.firstPersonLookSensitivity = firstPersonLookSensitivityFillImage.fillAmount * 300.0f;
        settingsMenuInstance.settings.invertY = invertYToggle.isOn;
    }

    public void ToggleButton(Toggle toggle)
    {
        audioManager.Play("SwitchMenuButton");
        toggle.isOn = !toggle.isOn;
    }

    public void SliderButtonCheck(GameObject slider, Image imageFill)
    {
        if (settingsMenuInstance.eventSystem.currentSelectedGameObject == slider)
        {
            float input = Input.GetAxisRaw("Horizontal");
            if (input >= 0.6f && slidingDelay >= maxSlidingDelay)
            {
                audioManager.Play("SwitchMenuButton");
                imageFill.fillAmount += 0.1f;
            }
            else if (input <= -0.6f && slidingDelay >= maxSlidingDelay)
            {
                audioManager.Play("SwitchMenuButton");
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
