using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuControllerControlsKeybinding : MonoBehaviour
{
    [HideInInspector] public GameObject firstSelectedButtonInController = null;
    [SerializeField] private GameObject pressAKeyCanvas = null;
    private GameObject currentSelectedGameObject = null;

    [HideInInspector] public CanvasGroup controllerControlsCanvasGroup = null;
    [SerializeField] Settings settings = null;
    private EventSystem eventSystem = null;

    public static PauseMenuControllerControlsKeybinding instance = null;
    private PauseMenuControlsSettings controlsSettingsMenuInstance = null;

    private Dictionary<string, KeyCode> keybindings = new Dictionary<string, KeyCode>();
    List<KeyCode> possibleKeyCodes = new List<KeyCode>();

    #region Buttons Images
    [SerializeField] private Image rotatePuzzleArrow = null;
    [SerializeField] private Image activateButtonInPuzzle = null;
    [SerializeField] private Image activatePuzzle = null;
    [SerializeField] private Image cameraToggle = null;
    [SerializeField] private Image movementToggle = null;
    [SerializeField] private Image disableGuard = null;
    [SerializeField] private Image hackGuard = null;
    [SerializeField] private Image distractGuardWhileHacking = null;
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

    private void Awake()
    {
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
        SetButtonKeySprite();
    }

    void Start()
    {
        controlsSettingsMenuInstance = PauseMenuControlsSettings.instance;
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && canTakeInput && changedKey)
        {
            controlsSettingsMenuInstance.StartSwitchingFromControllerControlsCoroutine();
            return;
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

        UpdateSettings();
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
