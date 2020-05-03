using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardControlsKeyBinding : MonoBehaviour
{
    [HideInInspector]
    public CanvasGroup keyboardControlsCanvasGroup = null;
    public static KeyboardControlsKeyBinding instance = null;
    [HideInInspector]
    public GameObject firstSelectedButtonInKeyboard = null;
    private ControlsSettingsMenu controlsSettingsMenuInstance = null;

    private Dictionary<string, KeyCode> keybindings = new Dictionary<string, KeyCode>();

    [SerializeField] private Text rotatePuzzleArrow;
    [SerializeField] private Text activateButtonInPuzzle;
    [SerializeField] private Text cameraToggle;
    [SerializeField] private Text movementToggle;
    [SerializeField] private Text disableGuard;
    [SerializeField] private Text hackGuard;
    [SerializeField] private Text distactGuardWhileHacking;

    [SerializeField] private GameObject pressAKeyCanvas;
    private GameObject currentSelectedGameObject;
    private EventSystem eventSystem = null;

    private bool canTakeInput = true;
    private bool changedKey = true;
    private Text changingKeyText;

    [SerializeField] private Settings settings;

    private void Awake()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        instance = this;
        keyboardControlsCanvasGroup = GetComponent<CanvasGroup>();
        keyboardControlsCanvasGroup.alpha = 0.0f;
        transform.gameObject.SetActive(false);
        pressAKeyCanvas.SetActive(false);
        firstSelectedButtonInKeyboard = transform.GetChild(0).gameObject;

        InitDictionaryKeys();
        SetButtonKeyText();
    }

    void Start()
    {
        controlsSettingsMenuInstance = ControlsSettingsMenu.instance;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && canTakeInput && changedKey)
        {
            controlsSettingsMenuInstance.StartSwitchingFromKeyboardControlsCoroutine();
            return;
        }

        UpdateSettings();
    }

    void InitDictionaryKeys()
    {
        //keybindings.Add("RotatePuzzleArrow", KeyCode.F);
        //keybindings.Add("ActivateButtonInPuzzle", KeyCode.E);
        //keybindings.Add("CameraToggle", KeyCode.Q);
        //keybindings.Add("MovementToggle", KeyCode.LeftShift);
        //keybindings.Add("DisableGuard", KeyCode.E);
        //keybindings.Add("HackGuard", KeyCode.R);
        //keybindings.Add("DistactGuardWhileHacking", KeyCode.E);
        
        keybindings.Add(settings.rotatePuzzleArrow, settings.rotatePuzzleArrowKeyboard);
        keybindings.Add(settings.activateButtonInPuzzle, settings.activateButtonInPuzzleKeyboard);
        keybindings.Add(settings.cameraToggle, settings.cameraToggleKeyboard);
        keybindings.Add(settings.movementToggle, settings.movementToggleKeyboard);
        keybindings.Add(settings.disableGuard, settings.disableGuardKeyboard);
        keybindings.Add(settings.hackGuard, settings.hackGuardKeyboard);
        keybindings.Add(settings.distractGuardWhileHacking, settings.distractGuardWhileHackingKeyboard);
    }

    void SetButtonKeyText()
    {
        rotatePuzzleArrow.text = keybindings[settings.rotatePuzzleArrow].ToString();
        activateButtonInPuzzle.text = keybindings[settings.activateButtonInPuzzle].ToString();
        cameraToggle.text = keybindings[settings.cameraToggle].ToString();
        movementToggle.text = keybindings[settings.movementToggle].ToString();
        disableGuard.text = keybindings[settings.disableGuard].ToString();
        hackGuard.text = keybindings[settings.hackGuard].ToString();
        distactGuardWhileHacking.text = keybindings[settings.distractGuardWhileHacking].ToString();
    }

    public void ChangeButton(Text buttonText)
    {
        StartCoroutine("ActivateChangeButtonPanel");
        changingKeyText = buttonText;
    }

    IEnumerator ActivateChangeButtonPanel()
    {
        currentSelectedGameObject = eventSystem.currentSelectedGameObject;
        pressAKeyCanvas.SetActive(true);
        canTakeInput = false;
        changedKey = false;
        yield return new WaitForSeconds(0.1f);
    }

    IEnumerator ChangeButtonText()
    {
            if (changedKey)
            {
                canTakeInput = false;
                yield return new WaitForSeconds(0.1f);
                pressAKeyCanvas.SetActive(false);
                canTakeInput = true;
            }
    }

    IEnumerator EnableInput()
    {
        yield return new WaitForSeconds(0.2f);
        canTakeInput = true;
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (!changedKey)
        {
            StartCoroutine("EnableInput");
            if (e.isKey && canTakeInput)
            {
                keybindings[currentSelectedGameObject.name] = e.keyCode;
                changingKeyText.text = e.keyCode.ToString();
                changedKey = true;
                changingKeyText = null;
                StartCoroutine("ChangeButtonText");
            }
        }
    }

    void UpdateSettings()
    {
        settings.rotatePuzzleArrowKeyboard = keybindings[settings.rotatePuzzleArrow];
        settings.activateButtonInPuzzleKeyboard = keybindings[settings.activateButtonInPuzzle];
        settings.cameraToggleKeyboard = keybindings[settings.cameraToggle];
        settings.movementToggleKeyboard = keybindings[settings.movementToggle];
        settings.disableGuardKeyboard = keybindings[settings.disableGuard];
        settings.hackGuardKeyboard = keybindings[settings.hackGuard];
        settings.distractGuardWhileHackingKeyboard = keybindings[settings.distractGuardWhileHacking];
    }
}
