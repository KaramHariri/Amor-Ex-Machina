using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PauseMenuControllerControlsKeybinding : MonoBehaviour
{
    [HideInInspector] public GameObject firstSelectedButtonInController = null;
    [SerializeField] private GameObject pressAKeyCanvas = null;
    private GameObject currentSelectedGameObject = null;

    [HideInInspector] public CanvasGroup controllerControlsCanvasGroup = null;
    private Settings settings = null;
    private EventSystem eventSystem = null;

    public static PauseMenuControllerControlsKeybinding instance = null;
    private PauseMenuControlsSettings controlsSettingsMenuInstance = null;

    private GameObject lastSelectedButton = null;

    private Dictionary<string, KeyCode> keybindings = new Dictionary<string, KeyCode>();
    List<KeyCode> possibleKeyCodes = new List<KeyCode>();

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

    #region Buttons Images
    private Image cameraToggle = null;
    private Image movementToggle = null;
    private Image disableGuard = null;
    private Image hackGuard = null;
    private Image distractGuardWhileHacking = null;
    private Image activatePuzzle = null;
    private Image activateButtonInPuzzle = null;
    private Image rotatePuzzleArrow = null;
    private Image changedKeyImage = null;
    #endregion

    private bool canTakeInput = true;
    private bool changedKey = true;

    #region Resources Sprites
    private Sprite squareSprite;
    private Sprite corssSprite;
    private Sprite circleSprite;
    private Sprite triangleSprite;
    private Sprite L1Sprite;
    private Sprite L2Sprite;
    private Sprite L3Sprite;
    private Sprite R1Sprite;
    private Sprite R2Sprite;
    private Sprite R3Sprite;
    #endregion

    private AudioManager audioManager = null;

    private void Awake()
    {
        settings = Resources.Load<Settings>("References/Settings/StaticSettings");
        InitPossibleKeyCodesList();

        LoadSprites();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        instance = this;
        controllerControlsCanvasGroup = GetComponent<CanvasGroup>();
        controllerControlsCanvasGroup.alpha = 0.0f;
        transform.gameObject.SetActive(false);
        pressAKeyCanvas.SetActive(false);

        firstSelectedButtonInController = transform.GetChild(0).gameObject;

        InitDictionaryKeys();
        InitButtonsGameObject();
        InitActionsText();
        InitButtonsImage();
        SetButtonKeySprite();
    }

    void Start()
    {
        controlsSettingsMenuInstance = PauseMenuControlsSettings.instance;
        audioManager = GameHandler.audioManager;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && canTakeInput && changedKey)
        {
            controlsSettingsMenuInstance.StartSwitchingFromControllerControlsCoroutine();
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

        for (int i = 0; i < possibleKeyCodes.Count; i++)
        {
            if (!changedKey)
            {
                StartCoroutine("EnableInput");
                if (canTakeInput && Input.GetKeyDown(possibleKeyCodes[i]))
                {
                    keybindings[currentSelectedGameObject.name] = possibleKeyCodes[i];
                    changedKeyImage.sprite = SetSprite(possibleKeyCodes[i]);
                    changedKey = true;
                    changedKeyImage = null;
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

    public void ResetKeys()
    {
        keybindings[settings.rotatePuzzleArrow] = settings.defaultRotatePuzzleArrowController;
        keybindings[settings.activateButtonInPuzzle] = settings.defaultActivateButtonInPuzzleController;
        keybindings[settings.activatePuzzle] = settings.defaultActivatePuzzleController;
        keybindings[settings.cameraToggle] = settings.defaultCameraToggleController;
        keybindings[settings.movementToggle] = settings.defaultMovementToggleController;
        keybindings[settings.disableGuard] = settings.defaultDisableGuardController;
        keybindings[settings.hackGuard] = settings.defaultHackGuardController;
        keybindings[settings.distractGuardWhileHacking] = settings.defaultDistractGuardWhileHackingController;

        SetButtonKeySprite();
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

    void InitButtonsImage()
    {
        cameraToggle = cameraToggleGameObject.transform.GetChild(1).GetComponent<Image>();
        movementToggle = movementToggleGameObject.transform.GetChild(1).GetComponent<Image>();
        disableGuard = disableGuardGameObject.transform.GetChild(1).GetComponent<Image>();
        hackGuard = hackGuardGameObject.transform.GetChild(1).GetComponent<Image>();
        distractGuardWhileHacking = distractGuardGameObject.transform.GetChild(1).GetComponent<Image>();
        activatePuzzle = activatePuzzleGameObject.transform.GetChild(1).GetComponent<Image>();
        activateButtonInPuzzle = activateButtonInPuzzleGameObject.transform.GetChild(1).GetComponent<Image>();
        rotatePuzzleArrow = rotateArrowInPuzzleGameObject.transform.GetChild(1).GetComponent<Image>();
    }

    void InitDictionaryKeys()
    {
        keybindings.Add(settings.rotatePuzzleArrow, settings.rotatePuzzleArrowController);
        keybindings.Add(settings.activateButtonInPuzzle, settings.activateButtonInPuzzleController);
        keybindings.Add(settings.activatePuzzle, settings.activatePuzzleController);
        keybindings.Add(settings.cameraToggle, settings.cameraToggleController);
        keybindings.Add(settings.movementToggle, settings.movementToggleController);
        keybindings.Add(settings.disableGuard, settings.disableGuardController);
        keybindings.Add(settings.hackGuard, settings.hackGuardController);
        keybindings.Add(settings.distractGuardWhileHacking, settings.distractGuardWhileHackingController);
    }

    void SetButtonKeySprite()
    {
        rotatePuzzleArrow.sprite = SetSprite(keybindings[settings.rotatePuzzleArrow]);
        activateButtonInPuzzle.sprite = SetSprite(keybindings[settings.activateButtonInPuzzle]);
        activatePuzzle.sprite = SetSprite(keybindings[settings.activatePuzzle]);
        cameraToggle.sprite = SetSprite(keybindings[settings.cameraToggle]);
        movementToggle.sprite = SetSprite(keybindings[settings.movementToggle]);
        disableGuard.sprite = SetSprite(keybindings[settings.disableGuard]);
        hackGuard.sprite = SetSprite(keybindings[settings.hackGuard]);
        distractGuardWhileHacking.sprite = SetSprite(keybindings[settings.distractGuardWhileHacking]);
    }

    Sprite SetSprite(KeyCode keyCode)
    {
        Sprite tempSprite = null;
        switch (keyCode)
        {
            case KeyCode.JoystickButton0:
                tempSprite = squareSprite;
                break;
            case KeyCode.JoystickButton1:
                tempSprite = corssSprite;
                break;
            case KeyCode.JoystickButton2:
                tempSprite = circleSprite;
                break;
            case KeyCode.JoystickButton3:
                tempSprite = triangleSprite;
                break;
            case KeyCode.JoystickButton4:
                tempSprite = L1Sprite;
                break;
            case KeyCode.JoystickButton5:
                tempSprite = R1Sprite;
                break;
            case KeyCode.JoystickButton6:
                tempSprite = L2Sprite;
                break;
            case KeyCode.JoystickButton7:
                tempSprite = R2Sprite;
                break;
            case KeyCode.JoystickButton10:
                tempSprite = L3Sprite;
                break;
            case KeyCode.JoystickButton11:
                tempSprite = R3Sprite;
                break;
            default:
                break;
        }
        return tempSprite;
    }

    void InitPossibleKeyCodesList()
    {
        possibleKeyCodes = new List<KeyCode>();
        possibleKeyCodes.Add(KeyCode.JoystickButton0);
        possibleKeyCodes.Add(KeyCode.JoystickButton1);
        possibleKeyCodes.Add(KeyCode.JoystickButton2);
        possibleKeyCodes.Add(KeyCode.JoystickButton3);
        possibleKeyCodes.Add(KeyCode.JoystickButton4);
        possibleKeyCodes.Add(KeyCode.JoystickButton5);
        possibleKeyCodes.Add(KeyCode.JoystickButton6);
        possibleKeyCodes.Add(KeyCode.JoystickButton7);
        possibleKeyCodes.Add(KeyCode.JoystickButton10);
        possibleKeyCodes.Add(KeyCode.JoystickButton11);
    }

    public void ChangeButton(Image buttonImage)
    {
        if (changedKey)
        {
            audioManager.Play("SwitchMenuButton");
            StartCoroutine("ActivateChangeButtonPanel");
            changedKeyImage = buttonImage;
        }
    }

    IEnumerator ActivateChangeButtonPanel()
    {
        currentSelectedGameObject = eventSystem.currentSelectedGameObject;
        pressAKeyCanvas.SetActive(true);
        canTakeInput = false;
        changedKey = false;
        yield return null;
    }

    IEnumerator ChangeButtonText()
    {
        if (changedKey)
        {
            canTakeInput = false;
            yield return null;
            pressAKeyCanvas.SetActive(false);
            canTakeInput = true;
        }
    }

    IEnumerator EnableInput()
    {
        yield return new WaitForSeconds(0.1f);
        canTakeInput = true;
    }

    void LoadSprites()
    {
        squareSprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/SquareButton");
        corssSprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/CrossButton");
        circleSprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/CircleButton");
        triangleSprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/TriangleButton");
        L1Sprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/L1Button");
        L2Sprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/L2Button");
        L3Sprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/L3Button");
        R1Sprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/R1Button");
        R2Sprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/R2Button");
        R3Sprite = Resources.Load<Sprite>("Graphics/PS4ControllerButtons/R3Button");
    }

    void UpdateSettings()
    {
        settings.rotatePuzzleArrowController = keybindings[settings.rotatePuzzleArrow];
        settings.activateButtonInPuzzleController = keybindings[settings.activateButtonInPuzzle];
        settings.activatePuzzleController = keybindings[settings.activatePuzzle];
        settings.cameraToggleController = keybindings[settings.cameraToggle];
        settings.movementToggleController = keybindings[settings.movementToggle];
        settings.disableGuardController = keybindings[settings.disableGuard];
        settings.hackGuardController = keybindings[settings.hackGuard];
        settings.distractGuardWhileHackingController = keybindings[settings.distractGuardWhileHacking];
    }
}
