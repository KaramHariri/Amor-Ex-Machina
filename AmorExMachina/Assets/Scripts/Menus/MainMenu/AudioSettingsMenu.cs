using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AudioSettingsMenu : MonoBehaviour
{
    private GameObject masterAudioSlider = null;
    private GameObject musicAudioSlider = null;
    private GameObject effectAudioSlider = null;
    private GameObject voiceAudioSlider = null;
    private GameObject footstepsAudioSlider = null;
    private GameObject back = null;

    private Image masterAudioFillImage = null;
    private Image effectAudioFillImage = null;
    private Image footstepsAudioFillImage = null;
    private Image voiceAudioFillImage = null;
    private Image musicAudioFillImage = null;

    private TextMeshProUGUI masterAudioAmountText = null;
    private TextMeshProUGUI effectAudioAmountText = null;
    private TextMeshProUGUI footstepsAudioAmountText = null;
    private TextMeshProUGUI voiceAudioAmountText = null;
    private TextMeshProUGUI musicAudioAmountText = null;

    private TextMeshProUGUI masterAudioText = null;
    private TextMeshProUGUI effectAudioText = null;
    private TextMeshProUGUI footstepsAudioText = null;
    private TextMeshProUGUI voiceAudioText = null;
    private TextMeshProUGUI musicAudioText = null;
    private TextMeshProUGUI backText = null;

    public static AudioSettingsMenu instance = null;
    [HideInInspector]
    public CanvasGroup audioCanvasGroup = null;
    [HideInInspector]
    public GameObject firstSelectedButtonInAudio = null;

    private OptionsMenu optionsMenuInstance = null;

    private float slidingDelay = 0.0f;
    private float maxSlidingDelay = 0.1f;

    [SerializeField] AudioSource buttonAudio = null;
    private Settings settings = null;
    private EventSystem eventSystem = null;

    private void Awake()
    {
        settings = Resources.Load<Settings>("References/Settings/StaticSettings");
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        instance = this;
        audioCanvasGroup = GetComponent<CanvasGroup>();
        audioCanvasGroup.alpha = 0.0f;
        transform.gameObject.SetActive(false);

        InitSliderGameObjects();
        InitSlidersFill();
        InitSlidersAmountText();
        InitSlidersText();

        firstSelectedButtonInAudio = masterAudioSlider;
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
        buttonAudio.volume = settings.effectsVolume * settings.masterVolume;

        SelectedButton();
        UpdateSlidersCheck();
        UpdateSettingsValues();
        UpdateSlidersAmountText();
    }

    void SelectedButton()
    {
        masterAudioText.color = Color.white;
        effectAudioText.color = Color.white;
        footstepsAudioText.color = Color.white;
        voiceAudioText.color = Color.white;
        musicAudioText.color = Color.white;
        backText.color = Color.white;

        if (eventSystem.currentSelectedGameObject == masterAudioSlider)
        {
            masterAudioText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == effectAudioSlider)
        {
            effectAudioText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == footstepsAudioSlider)
        {
            footstepsAudioText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == voiceAudioSlider)
        {
            voiceAudioText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
        }

        if (eventSystem.currentSelectedGameObject == musicAudioSlider)
        {
            musicAudioText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
        }

        if (eventSystem.currentSelectedGameObject == back)
        {
            backText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
        }
    }

    private void UpdateSlidersAmountText()
    {
        float masterAudioAmount = Mathf.RoundToInt(masterAudioFillImage.fillAmount * 100.0f);
        masterAudioAmountText.text = masterAudioAmount.ToString();
        float effectAudioAmount = Mathf.RoundToInt(effectAudioFillImage.fillAmount * 100.0f);
        effectAudioAmountText.text = effectAudioAmount.ToString();
        float footstepsAudioAmount = Mathf.RoundToInt(footstepsAudioFillImage.fillAmount * 100.0f);
        footstepsAudioAmountText.text = footstepsAudioAmount.ToString();
        float voiceAudioAmount = Mathf.RoundToInt(voiceAudioFillImage.fillAmount * 100.0f);
        voiceAudioAmountText.text = voiceAudioAmount.ToString();
        float musicAudioAmount = Mathf.RoundToInt(musicAudioFillImage.fillAmount * 100.0f);
        musicAudioAmountText.text = musicAudioAmount.ToString();
    }

    private void UpdateSlidersCheck()
    {
        SliderButtonCheck(masterAudioSlider, masterAudioFillImage);
        SliderButtonCheck(effectAudioSlider, effectAudioFillImage);
        SliderButtonCheck(footstepsAudioSlider, footstepsAudioFillImage);
        SliderButtonCheck(voiceAudioSlider, voiceAudioFillImage);
        SliderButtonCheck(musicAudioSlider, musicAudioFillImage);
    }

    private void InitSliderGameObjects()
    {
        masterAudioSlider = transform.GetChild(0).gameObject;
        musicAudioSlider = transform.GetChild(1).gameObject;
        voiceAudioSlider = transform.GetChild(2).gameObject;
        effectAudioSlider = transform.GetChild(3).gameObject;
        footstepsAudioSlider = transform.GetChild(4).gameObject;
        back = transform.GetChild(5).gameObject;
    }

    private void InitSlidersAmountText()
    {
        masterAudioAmountText = masterAudioSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        effectAudioAmountText = effectAudioSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        footstepsAudioAmountText = footstepsAudioSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        voiceAudioAmountText = voiceAudioSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        musicAudioAmountText = musicAudioSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }

    private void InitSlidersText()
    {
        masterAudioText = masterAudioSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        effectAudioText = effectAudioSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        footstepsAudioText = footstepsAudioSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        voiceAudioText = voiceAudioSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        musicAudioText = musicAudioSlider.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        backText = back.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void InitSlidersFill()
    {
        masterAudioFillImage = masterAudioSlider.transform.GetChild(masterAudioSlider.transform.childCount - 1).GetComponent<Image>();
        effectAudioFillImage = effectAudioSlider.transform.GetChild(effectAudioSlider.transform.childCount - 1).GetComponent<Image>();
        footstepsAudioFillImage = footstepsAudioSlider.transform.GetChild(footstepsAudioSlider.transform.childCount - 1).GetComponent<Image>();
        voiceAudioFillImage = voiceAudioSlider.transform.GetChild(voiceAudioSlider.transform.childCount - 1).GetComponent<Image>();
        musicAudioFillImage = musicAudioSlider.transform.GetChild(musicAudioSlider.transform.childCount - 1).GetComponent<Image>();
    }

    private void SetSettingsValues()
    {
        masterAudioFillImage.fillAmount = optionsMenuInstance.settings.masterVolume;
        effectAudioFillImage.fillAmount = optionsMenuInstance.settings.effectsVolume;
        footstepsAudioFillImage.fillAmount = optionsMenuInstance.settings.footstepsVolume;
        voiceAudioFillImage.fillAmount = optionsMenuInstance.settings.voiceVolume;
        musicAudioFillImage.fillAmount = optionsMenuInstance.settings.musicVolume;
    }

    private void UpdateSettingsValues()
    {
        optionsMenuInstance.settings.masterVolume = masterAudioFillImage.fillAmount;
        optionsMenuInstance.settings.effectsVolume = effectAudioFillImage.fillAmount;
        optionsMenuInstance.settings.footstepsVolume = footstepsAudioFillImage.fillAmount;
        optionsMenuInstance.settings.voiceVolume = voiceAudioFillImage.fillAmount;
        optionsMenuInstance.settings.musicVolume = musicAudioFillImage.fillAmount;
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

            if(input == 0.0f)
            {
                slidingDelay = maxSlidingDelay;
            }
        }
    }
}
