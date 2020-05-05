using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardControlsKeyBinding : MonoBehaviour
{
    [HideInInspector] public CanvasGroup keyboardControlsCanvasGroup = null;
    [HideInInspector] public GameObject firstSelectedButtonInKeyboard = null;
    [SerializeField] private GameObject pressAKeyCanvas = null;
    private GameObject currentSelectedGameObject = null;

    private EventSystem eventSystem = null;
    [SerializeField] private Settings settings = null;

    private ControlsSettingsMenu controlsSettingsMenuInstance = null;
    public static KeyboardControlsKeyBinding instance = null;

    private Dictionary<string, KeyCode> keybindings = new Dictionary<string, KeyCode>();

    #region Buttons Text
    [SerializeField] private Text rotatePuzzleArrow = null;
    [SerializeField] private Text activateButtonInPuzzle = null;
    [SerializeField] private Text activatePuzzle = null;
    [SerializeField] private Text cameraToggle = null;
    [SerializeField] private Text movementToggle = null;
    [SerializeField] private Text disableGuard = null;
    [SerializeField] private Text hackGuard = null;
    [SerializeField] private Text distractGuardWhileHacking = null;
    private Text changedKeyText = null;
    #endregion

    private bool canTakeInput = true;
    private bool changedKey = true;


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

        if (!changedKey)
        {
            StartCoroutine("EnableInput");
            if (canTakeInput)
            {
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    keybindings[currentSelectedGameObject.name] = KeyCode.LeftShift;
                    changedKeyText.text = KeyCode.LeftShift.ToString();
                    changedKey = true;
                    changedKeyText = null;
                    StartCoroutine("ChangeButtonText");
                }
                else if(Input.GetKeyDown(KeyCode.RightShift))
                {
                    keybindings[currentSelectedGameObject.name] = KeyCode.RightShift;
                    changedKeyText.text = KeyCode.RightShift.ToString();
                    changedKey = true;
                    changedKeyText = null;
                    StartCoroutine("ChangeButtonText");
                }
            }
        }

        UpdateSettings();
    }

    void InitDictionaryKeys()
    {    
        keybindings.Add(settings.rotatePuzzleArrow, settings.rotatePuzzleArrowKeyboard);
        keybindings.Add(settings.activateButtonInPuzzle, settings.activateButtonInPuzzleKeyboard);
        keybindings.Add(settings.activatePuzzle, settings.activatePuzzleKeyboard);
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
        activatePuzzle.text = keybindings[settings.activatePuzzle].ToString();
        cameraToggle.text = keybindings[settings.cameraToggle].ToString();
        movementToggle.text = keybindings[settings.movementToggle].ToString();
        disableGuard.text = keybindings[settings.disableGuard].ToString();
        hackGuard.text = keybindings[settings.hackGuard].ToString();
        distractGuardWhileHacking.text = keybindings[settings.distractGuardWhileHacking].ToString();
    }

    public void ResetKeys()
    {
        keybindings[settings.rotatePuzzleArrow] = settings.defaultRotatePuzzleArrowKeyboard;
        keybindings[settings.activateButtonInPuzzle] = settings.defaultActivateButtonInPuzzleKeyboard;
        keybindings[settings.activatePuzzle] = settings.defaultActivatePuzzleKeyboard;
        keybindings[settings.cameraToggle] = settings.defaultCameraToggleKeyboard;
        keybindings[settings.movementToggle] = settings.defaultMovementToggleKeyboard;
        keybindings[settings.disableGuard] = settings.defaultDisableGuardKeyboard;
        keybindings[settings.hackGuard] = settings.defaultHackGuardKeyboard;
        keybindings[settings.distractGuardWhileHacking] = settings.defaultDistractGuardWhileHackingKeyboard;

        SetButtonKeyText();
    }

    public void ChangeButton(Text buttonText)
    {
        StartCoroutine("ActivateChangeButtonPanel");
        changedKeyText = buttonText;
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
                changedKeyText.text = e.keyCode.ToString();
                changedKey = true;
                changedKeyText = null;
                StartCoroutine("ChangeButtonText");
            }
        }
    }

    void UpdateSettings()
    {
        settings.rotatePuzzleArrowKeyboard = keybindings[settings.rotatePuzzleArrow];
        settings.activateButtonInPuzzleKeyboard = keybindings[settings.activateButtonInPuzzle];
        settings.activatePuzzleKeyboard = keybindings[settings.activatePuzzle];
        settings.cameraToggleKeyboard = keybindings[settings.cameraToggle];
        settings.movementToggleKeyboard = keybindings[settings.movementToggle];
        settings.disableGuardKeyboard = keybindings[settings.disableGuard];
        settings.hackGuardKeyboard = keybindings[settings.hackGuard];
        settings.distractGuardWhileHackingKeyboard = keybindings[settings.distractGuardWhileHacking];
    }
}
