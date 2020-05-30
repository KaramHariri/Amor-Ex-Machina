using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ControlsSettingsMenu : MonoBehaviour
{
    [HideInInspector]
    public CanvasGroup controlsCanvasGroup = null;
    private GameObject controlsButtonHolder = null;
    
    public GameObject firstSelectedButtonInControls = null;
    public static ControlsSettingsMenu instance = null;

    private OptionsMenu optionsMenuInstance = null;
    private bool canTakeInput = true;
   
    GameObject currentSelectedButton = null;
    public float fadingSpeed = 4.0f;

    private KeyboardControlsKeyBinding keyboardKeyBinding = null;
    private ControllerControlsKeyBinding controllerKeyBinding = null;

    [SerializeField] private Button controllerControlsButton = null;
    [SerializeField] private Button keyboardControlsButton = null;
    [SerializeField] private Button backButton = null;

    private TextMeshProUGUI controllerText = null;
    private TextMeshProUGUI keyboardText = null;
    private TextMeshProUGUI backText = null;

    private EventSystem eventSystem = null;

    private void Awake()
    {
        instance = this;
        controlsCanvasGroup = GetComponent<CanvasGroup>();
        controlsCanvasGroup.alpha = 0.0f;

        controlsButtonHolder = transform.GetChild(0).gameObject;

        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    private void Start()
    {
        optionsMenuInstance = OptionsMenu.instance;
        keyboardKeyBinding = KeyboardControlsKeyBinding.instance;
        controllerKeyBinding = ControllerControlsKeyBinding.instance;
        transform.gameObject.SetActive(false);
        keyboardKeyBinding.gameObject.SetActive(false);
        controllerKeyBinding.gameObject.SetActive(false);

        InitButtonsText();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && canTakeInput)
        {
            optionsMenuInstance.StartSwitchingFromControlsCoroutine();
            return;
        }

        SelectedButton();
        SetButtonsInteractable();
    }

    void SelectedButton()
    {
        controllerText.color = Color.white;
        keyboardText.color = Color.white;
        backText.color = Color.white;

        if (eventSystem.currentSelectedGameObject == controllerControlsButton.gameObject)
        {
            controllerText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == keyboardControlsButton.gameObject)
        {
            keyboardText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == backButton.gameObject)
        {
            backText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }
    }

    void InitButtonsText()
    {
        controllerText = controllerControlsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        keyboardText = keyboardControlsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        backText = backButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void SetButtonsInteractable()
    {
        if (!optionsMenuInstance.settings.useControllerInput)
        {
            //keyboardControlsButton.interactable = true;
            controllerControlsButton.interactable = false;
        }
        else
        {
            //keyboardControlsButton.interactable = false;
            controllerControlsButton.interactable = true;
        }
    }

    public IEnumerator UpdateCurrentSelectedObject(GameObject nextSelectedObject)
    {
        currentSelectedButton = optionsMenuInstance.eventSystem.currentSelectedGameObject;
        optionsMenuInstance.eventSystem.SetSelectedGameObject(null);
        yield return null;
        optionsMenuInstance.eventSystem.SetSelectedGameObject(nextSelectedObject);
    }

    public void StartSwitchingToKeyboardControlsCoroutine()
    {
        keyboardKeyBinding.keyboardControlsCanvasGroup.gameObject.SetActive(true);
        controlsButtonHolder.gameObject.SetActive(false);
        controllerKeyBinding.controllerControlsCanvasGroup.gameObject.SetActive(false);
        canTakeInput = false;
        StartCoroutine(UpdateCurrentSelectedObject(keyboardKeyBinding.firstSelectedButtonInKeyboard));
        StartCoroutine(optionsMenuInstance.SwitchOptionMenu(optionsMenuInstance.FadeOutCanvasGroup(controlsCanvasGroup),
                                                            optionsMenuInstance.FadeInCanvasGroup(keyboardKeyBinding.keyboardControlsCanvasGroup)));
    }

    public void StartSwitchingToControllerControlsCoroutine()
    {
        controllerKeyBinding.controllerControlsCanvasGroup.gameObject.SetActive(true);
        keyboardKeyBinding.keyboardControlsCanvasGroup.gameObject.SetActive(false);
        controlsButtonHolder.SetActive(false);
        canTakeInput = false;
        StartCoroutine(UpdateCurrentSelectedObject(controllerKeyBinding.firstSelectedButtonInController));
        StartCoroutine(optionsMenuInstance.SwitchOptionMenu(optionsMenuInstance.FadeOutCanvasGroup(controlsCanvasGroup),
                                                            optionsMenuInstance.FadeInCanvasGroup(controllerKeyBinding.controllerControlsCanvasGroup)));
    }

    public void StartSwitchingFromKeyboardControlsCoroutine()
    {
        controlsCanvasGroup.gameObject.SetActive(true);
        controlsButtonHolder.SetActive(true);
        keyboardKeyBinding.keyboardControlsCanvasGroup.gameObject.SetActive(false);
        optionsMenuInstance.eventSystem.SetSelectedGameObject(null);
        optionsMenuInstance.eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(optionsMenuInstance.SwitchOptionMenu(optionsMenuInstance.FadeOutCanvasGroup(keyboardKeyBinding.keyboardControlsCanvasGroup),
                                                            optionsMenuInstance.FadeInCanvasGroup(controlsCanvasGroup)));
        canTakeInput = true;
    }

    public void StartSwitchingFromControllerControlsCoroutine()
    {
        controlsButtonHolder.SetActive(true);
        controllerKeyBinding.controllerControlsCanvasGroup.gameObject.SetActive(false);
        optionsMenuInstance.eventSystem.SetSelectedGameObject(null);
        optionsMenuInstance.eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(optionsMenuInstance.SwitchOptionMenu(optionsMenuInstance.FadeOutCanvasGroup(controllerKeyBinding.controllerControlsCanvasGroup),
                                                            optionsMenuInstance.FadeInCanvasGroup(controlsCanvasGroup)));
        canTakeInput = true;
    }
}
