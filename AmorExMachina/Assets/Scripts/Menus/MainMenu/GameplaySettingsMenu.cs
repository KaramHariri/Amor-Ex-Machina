using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameplaySettingsMenu : MonoBehaviour
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

    public static GameplaySettingsMenu instance;
    [HideInInspector]
    public CanvasGroup gameplayCanvasGroup = null;
    [HideInInspector]
    public GameObject firstSelectedButtonInGameplay = null;

    private OptionsMenu optionsMenuInstance = null;

    private float slidingDelay = 0.0f;
    private float maxSlidingDelay = 0.1f;

    [SerializeField] AudioSource buttonAudio;
    EventSystem eventSystem = null;

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

        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
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

        buttonAudio.volume = optionsMenuInstance.settings.effectsVolume * optionsMenuInstance.settings.masterVolume;

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
            invertYText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == thirdPersonLookSensitivitySlider.gameObject)
        {
            thirdPersonLookSensitivityText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == firstPersonLookSensitivitySlider.gameObject)
        {
            firstPersonLookSensitivityText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == backGameObject.gameObject)
        {
            backText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
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
        thirdPersonLookSensitivityFillImage.fillAmount = optionsMenuInstance.settings.thirdPersonLookSensitivity / 300.0f;
        firstPersonLookSensitivityFillImage.fillAmount = optionsMenuInstance.settings.firstPersonLookSensitivity / 300.0f;
        invertYToggle.isOn = optionsMenuInstance.settings.invertY;
    }

    private void UpdateSettingsValues()
    {
        optionsMenuInstance.settings.thirdPersonLookSensitivity = thirdPersonLookSensitivityFillImage.fillAmount * 300.0f;
        optionsMenuInstance.settings.firstPersonLookSensitivity = firstPersonLookSensitivityFillImage.fillAmount * 300.0f;
        optionsMenuInstance.settings.invertY = invertYToggle.isOn;
    }

    public void ToggleButton(Toggle toggle)
    {
        buttonAudio.Play();
        toggle.isOn = !toggle.isOn;
    }

    public void SliderButtonCheck(GameObject slider, Image imageFill)
    {
        if (optionsMenuInstance.eventSystem.currentSelectedGameObject == slider)
        {
            float input = Input.GetAxisRaw("Horizontal");
            if (input >= 0.6f && slidingDelay >= maxSlidingDelay)
            {
                buttonAudio.Play();
                imageFill.fillAmount += 0.1f;
            }
            else if (input <= -0.6f && slidingDelay >= maxSlidingDelay)
            {
                buttonAudio.Play();
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
