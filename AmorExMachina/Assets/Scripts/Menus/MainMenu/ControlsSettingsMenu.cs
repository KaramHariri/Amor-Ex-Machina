using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Awake()
    {
        instance = this;
        controlsCanvasGroup = GetComponent<CanvasGroup>();
        controlsCanvasGroup.alpha = 0.0f;

        controlsButtonHolder = transform.GetChild(0).gameObject;
    }

    private void Start()
    {
        optionsMenuInstance = OptionsMenu.instance;
        keyboardKeyBinding = KeyboardControlsKeyBinding.instance;
        controllerKeyBinding = ControllerControlsKeyBinding.instance;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && canTakeInput)
        {
            optionsMenuInstance.StartSwitchingFromControlsCoroutine();
            return;
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
