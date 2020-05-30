using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] MainMenu mainMenu = null;

    public GameObject firstSelectedButtonInOptions = null;
    private GameObject currentSelectedButton = null;

    public EventSystem eventSystem = null;
    public CanvasGroup buttonsCanvasGroup = null;
    [HideInInspector] public Settings settings = null;

    private AudioSettingsMenu audioSettingsMenuInstance = null;
    private GameplaySettingsMenu gameplaySettingsMenuInstance = null;
    private ControlsSettingsMenu controlsSettingsMenuInstance = null;
    public static OptionsMenu instance = null;

    public float fadingSpeed = 4.0f;
    private bool canTakeInput = true;

    [SerializeField] private GameObject cameraSettings = null;
    [SerializeField] private GameObject audioSettings = null;
    [SerializeField] private GameObject controlsSettings = null;
    [SerializeField] private GameObject back = null;

    private TextMeshProUGUI cameraSettingsText = null;
    private TextMeshProUGUI audioSettingsText = null;
    private TextMeshProUGUI controlsSettingsText = null;
    private TextMeshProUGUI backText = null;

    private void Awake()
    {
        instance = this;
        transform.gameObject.SetActive(false);
        settings = Resources.Load<Settings>("References/Settings/StaticSettings");
    }

    private void Start()
    {
        audioSettingsMenuInstance = AudioSettingsMenu.instance;
        gameplaySettingsMenuInstance = GameplaySettingsMenu.instance;
        controlsSettingsMenuInstance = ControlsSettingsMenu.instance;

        cameraSettingsText = cameraSettings.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        audioSettingsText = audioSettings.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        controlsSettingsText = controlsSettings.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        backText = back.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && canTakeInput)
        {
            ExitOptionsMenu();
        }

        SelectedButton();
    }

    void SelectedButton()
    {
        cameraSettingsText.color = Color.white;
        audioSettingsText.color = Color.white;
        controlsSettingsText.color = Color.white;
        backText.color = Color.white;

        if (eventSystem.currentSelectedGameObject == cameraSettings.gameObject)
        {
            cameraSettingsText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == audioSettings.gameObject)
        {
            audioSettingsText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == controlsSettings.gameObject)
        {
            controlsSettingsText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == back.gameObject)
        {
            backText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
        }
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
        mainMenu.transform.GetChild(0).gameObject.SetActive(true);
        yield return null;
        transform.gameObject.SetActive(false);
        mainMenu.StartSetSelectedButtonEnumerator();
    }

    public void StartSwitchingToAudioMenuCoroutine()
    {
        audioSettingsMenuInstance.audioCanvasGroup.gameObject.SetActive(true);
        gameplaySettingsMenuInstance.gameplayCanvasGroup.gameObject.SetActive(false);
        controlsSettingsMenuInstance.controlsCanvasGroup.gameObject.SetActive(false);
        buttonsCanvasGroup.gameObject.SetActive(false);
        canTakeInput = false;
        StartCoroutine(UpdateCurrentSelectedObject(audioSettingsMenuInstance.firstSelectedButtonInAudio));
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(buttonsCanvasGroup), FadeInCanvasGroup(audioSettingsMenuInstance.audioCanvasGroup)));
    }

    public void StartSwitchingToGameplayCoroutine()
    {
        gameplaySettingsMenuInstance.gameplayCanvasGroup.gameObject.SetActive(true);
        audioSettingsMenuInstance.audioCanvasGroup.gameObject.SetActive(false);
        controlsSettingsMenuInstance.controlsCanvasGroup.gameObject.SetActive(false);
        buttonsCanvasGroup.gameObject.SetActive(false);
        canTakeInput = false;
        StartCoroutine(UpdateCurrentSelectedObject(gameplaySettingsMenuInstance.firstSelectedButtonInGameplay));
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(buttonsCanvasGroup), FadeInCanvasGroup(gameplaySettingsMenuInstance.gameplayCanvasGroup)));
    }

    public void StartSwitchingToControlsCouroutine()
    {
        controlsSettingsMenuInstance.controlsCanvasGroup.gameObject.SetActive(true);
        audioSettingsMenuInstance.audioCanvasGroup.gameObject.SetActive(false);
        gameplaySettingsMenuInstance.gameplayCanvasGroup.gameObject.SetActive(false);
        buttonsCanvasGroup.gameObject.SetActive(false);
        canTakeInput = false;
        StartCoroutine(UpdateCurrentSelectedObject(controlsSettingsMenuInstance.firstSelectedButtonInControls));
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(buttonsCanvasGroup), FadeInCanvasGroup(controlsSettingsMenuInstance.controlsCanvasGroup)));
    }

    public void StartSwitchingFromAudioCoroutine()
    {
        buttonsCanvasGroup.gameObject.SetActive(true);
        audioSettingsMenuInstance.audioCanvasGroup.gameObject.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(audioSettingsMenuInstance.audioCanvasGroup), FadeInCanvasGroup(buttonsCanvasGroup)));
        canTakeInput = true;
    }

    public void StartSwitchingFromGameplayCoroutine()
    {
        buttonsCanvasGroup.gameObject.SetActive(true);
        gameplaySettingsMenuInstance.gameplayCanvasGroup.gameObject.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(gameplaySettingsMenuInstance.gameplayCanvasGroup), FadeInCanvasGroup(buttonsCanvasGroup)));
        canTakeInput = true;
    }

    public void StartSwitchingFromControlsCoroutine()
    {
        buttonsCanvasGroup.gameObject.SetActive(true);
        controlsSettingsMenuInstance.controlsCanvasGroup.gameObject.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(SwitchOptionMenu(FadeOutCanvasGroup(controlsSettingsMenuInstance.controlsCanvasGroup), FadeInCanvasGroup(buttonsCanvasGroup)));
        canTakeInput = true;
    }

    public IEnumerator SwitchOptionMenu(IEnumerator fadeOutEnumerator, IEnumerator fadeInEnumerator)
    {
        StartCoroutine(fadeOutEnumerator);
        yield return null;
        StartCoroutine(fadeInEnumerator);
    }

    public IEnumerator FadeOutCanvasGroup(CanvasGroup canvasGroupToFade)
    {
        while(canvasGroupToFade.alpha > 0.0f)
        {
            canvasGroupToFade.alpha -= Time.deltaTime * fadingSpeed;
            yield return null;
        }
    }

    public IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroupToFade)
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
}
