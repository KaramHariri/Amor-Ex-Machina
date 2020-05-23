using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenuKeyboardControlsKeybinding : MonoBehaviour
{
    [HideInInspector] public CanvasGroup keyboardControlsCanvasGroup = null;
    [HideInInspector] public GameObject firstSelectedButtonInKeyboard = null;
    private GameObject lastSelectedButton = null;
    [SerializeField] private GameObject pressAKeyCanvas = null;
    private GameObject currentSelectedGameObject = null;

    private EventSystem eventSystem = null;
    private Settings settings = null;

    private PauseMenuControlsSettings controlsSettingsMenuInstance = null;
    public static PauseMenuKeyboardControlsKeybinding instance = null;

    private Dictionary<string, KeyCode> keybindings = new Dictionary<string, KeyCode>();

    #region GameObjects
    private GameObject cameraToggleGameObject = null;
    private GameObject movementToggleGameObject = null;
    private GameObject disableGuardGameObject = null;
    private GameObject hackGuardGameObject = null;
    private GameObject distractGuardGameObject = null;
    private GameObject activatePuzzleGameObject = null;
    private GameObject activateButtonInPuzzleGameObject = null;
    private GameObject rotateArrowInPuzzleGameObject = null;
    private GameObject useDefaultGameObject = null;
    private GameObject backGameObject = null;

    private TextMeshProUGUI cameraToggleActionText = null;
    private TextMeshProUGUI movementToggleActionText = null;
    private TextMeshProUGUI disableGuardActionText = null;
    private TextMeshProUGUI hackGuardActionText = null;
    private TextMeshProUGUI distractGuardActionText = null;
    private TextMeshProUGUI activatePuzzleActionText = null;
    private TextMeshProUGUI activateButtonInPuzzleActionText = null;
    private TextMeshProUGUI rotateArrowInPuzzleActionText = null;
    private TextMeshProUGUI useDefaultText = null;
    private TextMeshProUGUI backText = null;
    #endregion

    #region Buttons Text
    private TextMeshProUGUI cameraToggle = null;
    private TextMeshProUGUI movementToggle = null;
    private TextMeshProUGUI disableGuard = null;
    private TextMeshProUGUI hackGuard = null;
    private TextMeshProUGUI distractGuardWhileHacking = null;
    private TextMeshProUGUI activatePuzzle = null;
    private TextMeshProUGUI activateButtonInPuzzle = null;
    private TextMeshProUGUI rotatePuzzleArrow = null;
    private TextMeshProUGUI changedKeyText = null;
    #endregion

    private bool canTakeInput = true;
    private bool changedKey = true;

    private AudioManager audioManager = null;

    private void Awake()
    {
        settings = Resources.Load<Settings>("References/Settings/StaticSettings");
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        instance = this;
        keyboardControlsCanvasGroup = GetComponent<CanvasGroup>();
        keyboardControlsCanvasGroup.alpha = 0.0f;
        transform.gameObject.SetActive(false);
        pressAKeyCanvas.SetActive(false);
        firstSelectedButtonInKeyboard = transform.GetChild(0).gameObject;

        InitDictionaryKeys();
        InitButtonsGameObject();
        InitActionsText();
        InitButtonText();
        SetButtonKeyText();
    }

    void Start()
    {
        controlsSettingsMenuInstance = PauseMenuControlsSettings.instance;
        audioManager = GameHandler.audioManager;
    }

    void InitButtonsGameObject()
    {
        cameraToggleGameObject = transform.GetChild(0).gameObject;
        movementToggleGameObject = transform.GetChild(1).gameObject;
        disableGuardGameObject = transform.GetChild(2).gameObject;
        hackGuardGameObject = transform.GetChild(3).gameObject;
        distractGuardGameObject = transform.GetChild(4).gameObject;
        activatePuzzleGameObject = transform.GetChild(5).gameObject;
        activateButtonInPuzzleGameObject = transform.GetChild(6).gameObject;
        rotateArrowInPuzzleGameObject = transform.GetChild(7).gameObject;
        useDefaultGameObject = transform.GetChild(8).gameObject;
        backGameObject = transform.GetChild(9).gameObject;
    }

    void InitActionsText()
    {
        cameraToggleActionText = cameraToggleGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        movementToggleActionText = movementToggleGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        disableGuardActionText = disableGuardGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        hackGuardActionText = hackGuardGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        distractGuardActionText = distractGuardGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        activatePuzzleActionText = activatePuzzleGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        activateButtonInPuzzleActionText = activateButtonInPuzzleGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        rotateArrowInPuzzleActionText = rotateArrowInPuzzleGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        useDefaultText = useDefaultGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        backText = backGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    void InitButtonText()
    {
        cameraToggle = cameraToggleGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        movementToggle = movementToggleGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        disableGuard = disableGuardGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        hackGuard = hackGuardGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        distractGuardWhileHacking = distractGuardGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        activatePuzzle = activatePuzzleGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        activateButtonInPuzzle = activateButtonInPuzzleGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        rotatePuzzleArrow = rotateArrowInPuzzleGameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && canTakeInput && changedKey)
        {
            controlsSettingsMenuInstance.StartSwitchingFromKeyboardControlsCoroutine();
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
                else if (Input.GetKeyDown(KeyCode.RightShift))
                {
                    keybindings[currentSelectedGameObject.name] = KeyCode.RightShift;
                    changedKeyText.text = KeyCode.RightShift.ToString();
                    changedKey = true;
                    changedKeyText = null;
                    StartCoroutine("ChangeButtonText");
                }
            }
        }
        SelectedButton();
        UpdateSettings();
    }

    void SelectedButton()
    {
        cameraToggleActionText.color = Color.white;
        movementToggleActionText.color = Color.white;
        disableGuardActionText.color = Color.white;
        hackGuardActionText.color = Color.white;
        distractGuardActionText.color = Color.white;
        activatePuzzleActionText.color = Color.white;
        activateButtonInPuzzleActionText.color = Color.white;
        rotateArrowInPuzzleActionText.color = Color.white;
        useDefaultText.color = Color.white;
        backText.color = Color.white;

        if (eventSystem.currentSelectedGameObject == cameraToggleGameObject.gameObject)
        {
            cameraToggleActionText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == movementToggleGameObject.gameObject)
        {
            movementToggleActionText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == disableGuardGameObject.gameObject)
        {
            disableGuardActionText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == hackGuardGameObject.gameObject)
        {
            hackGuardActionText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
        }

        if (eventSystem.currentSelectedGameObject == distractGuardGameObject.gameObject)
        {
            distractGuardActionText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
        }

        if (eventSystem.currentSelectedGameObject == activatePuzzleGameObject.gameObject)
        {
            activatePuzzleActionText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
        }

        if (eventSystem.currentSelectedGameObject == activateButtonInPuzzleGameObject.gameObject)
        {
            activateButtonInPuzzleActionText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
        }

        if (eventSystem.currentSelectedGameObject == rotateArrowInPuzzleGameObject.gameObject)
        {
            rotateArrowInPuzzleActionText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
        }

        if (eventSystem.currentSelectedGameObject == useDefaultGameObject.gameObject)
        {
            useDefaultText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
        }

        if (eventSystem.currentSelectedGameObject == backGameObject.gameObject)
        {
            backText.color = new Color(1.0f, 0.8156863f, 0.08627451f, 1.0f);
        }
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

    public void ChangeButton(TextMeshProUGUI buttonText)
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
            audioManager.Play("SwitchMenuButton");
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
