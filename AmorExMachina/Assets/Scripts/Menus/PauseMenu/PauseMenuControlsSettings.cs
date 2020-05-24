using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenuControlsSettings : MonoBehaviour
{
    [HideInInspector]
    public CanvasGroup controlsCanvasGroup = null;
    private GameObject controlsButtonHolder = null;

    public GameObject firstSelectedButtonInControls = null;
    public static PauseMenuControlsSettings instance = null;

    private PauseMenuSettings settingsMenuInstance = null;
    private bool canTakeInput = true;

    GameObject currentSelectedButton = null;
    public float fadingSpeed = 4.0f;

    private PauseMenuKeyboardControlsKeybinding keyboardKeyBinding = null;
    private PauseMenuControllerControlsKeybinding controllerKeyBinding = null;

    [SerializeField] private Button controllerControlsButton = null;
    [SerializeField] private Button keyboardControlsButton = null;
    [SerializeField] private Button backButton = null;

    private TextMeshProUGUI controllerText = null;
    private TextMeshProUGUI keyboardText = null;
    private TextMeshProUGUI backText = null;

    private GameObject lastSelectedButton = null;
    private EventSystem eventSystem = null;

    private void Awake()
    {
        instance = this;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        controlsCanvasGroup = GetComponent<CanvasGroup>();
        controlsCanvasGroup.alpha = 0.0f;

        controlsButtonHolder = transform.GetChild(0).gameObject;
        InitButtonsText();
    }

    private void Start()
    {
        settingsMenuInstance = PauseMenuSettings.instance;
        keyboardKeyBinding = PauseMenuKeyboardControlsKeybinding.instance;
        controllerKeyBinding = PauseMenuControllerControlsKeybinding.instance;
        transform.gameObject.SetActive(false);
        keyboardKeyBinding.gameObject.SetActive(false);
        controllerKeyBinding.gameObject.SetActive(false);
    }

    void InitButtonsText()
    {
        controllerText = controllerControlsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        keyboardText = keyboardControlsButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        backText = backButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && canTakeInput)
        {
            settingsMenuInstance.StartSwitchingFromControlsCoroutine();
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
        SetButtonsInteractable();
    }

    void SelectedButton()
    {
        controllerText.color = Color.white;
        keyboardText.color = Color.white;
        backText.color = Color.white;

        if (eventSystem.currentSelectedGameObject == controllerControlsButton.gameObject)
        {
            controllerText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == keyboardControlsButton.gameObject)
        {
            keyboardText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == backButton.gameObject)
        {
            backText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
            return;
        }
    }

    void SetButtonsInteractable()
    {
        if (!settingsMenuInstance.settings.useControllerInput)
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
        currentSelectedButton = settingsMenuInstance.eventSystem.currentSelectedGameObject;
        settingsMenuInstance.eventSystem.SetSelectedGameObject(null);
        yield return null;
        settingsMenuInstance.eventSystem.SetSelectedGameObject(nextSelectedObject);
    }

    public void StartSwitchingToKeyboardControlsCoroutine()
    {
        keyboardKeyBinding.keyboardControlsCanvasGroup.gameObject.SetActive(true);
        controlsButtonHolder.gameObject.SetActive(false);
        controllerKeyBinding.controllerControlsCanvasGroup.gameObject.SetActive(false);
        canTakeInput = false;
        StartCoroutine(UpdateCurrentSelectedObject(keyboardKeyBinding.firstSelectedButtonInKeyboard));
        StartCoroutine(settingsMenuInstance.SwitchOptionMenu(settingsMenuInstance.FadeOutCanvasGroup(controlsCanvasGroup),
                                                            settingsMenuInstance.FadeInCanvasGroup(keyboardKeyBinding.keyboardControlsCanvasGroup)));
    }

    public void StartSwitchingToControllerControlsCoroutine()
    {
        controllerKeyBinding.controllerControlsCanvasGroup.gameObject.SetActive(true);
        keyboardKeyBinding.keyboardControlsCanvasGroup.gameObject.SetActive(false);
        controlsButtonHolder.SetActive(false);
        canTakeInput = false;
        StartCoroutine(UpdateCurrentSelectedObject(controllerKeyBinding.firstSelectedButtonInController));
        StartCoroutine(settingsMenuInstance.SwitchOptionMenu(settingsMenuInstance.FadeOutCanvasGroup(controlsCanvasGroup),
                                                            settingsMenuInstance.FadeInCanvasGroup(controllerKeyBinding.controllerControlsCanvasGroup)));
    }

    public void StartSwitchingFromKeyboardControlsCoroutine()
    {
        controlsCanvasGroup.gameObject.SetActive(true);
        controlsButtonHolder.SetActive(true);
        keyboardKeyBinding.keyboardControlsCanvasGroup.gameObject.SetActive(false);
        settingsMenuInstance.eventSystem.SetSelectedGameObject(null);
        settingsMenuInstance.eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(settingsMenuInstance.SwitchOptionMenu(settingsMenuInstance.FadeOutCanvasGroup(keyboardKeyBinding.keyboardControlsCanvasGroup),
                                                            settingsMenuInstance.FadeInCanvasGroup(controlsCanvasGroup)));
        canTakeInput = true;
    }

    public void StartSwitchingFromControllerControlsCoroutine()
    {
        controlsButtonHolder.SetActive(true);
        controllerKeyBinding.controllerControlsCanvasGroup.gameObject.SetActive(false);
        settingsMenuInstance.eventSystem.SetSelectedGameObject(null);
        settingsMenuInstance.eventSystem.SetSelectedGameObject(currentSelectedButton);
        StartCoroutine(settingsMenuInstance.SwitchOptionMenu(settingsMenuInstance.FadeOutCanvasGroup(controllerKeyBinding.controllerControlsCanvasGroup),
                                                            settingsMenuInstance.FadeInCanvasGroup(controlsCanvasGroup)));
        canTakeInput = true;
    }
}
