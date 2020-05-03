using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControllerControlsKeyBinding : MonoBehaviour
{
    [HideInInspector]
    public CanvasGroup controllerControlsCanvasGroup = null;
    public static ControllerControlsKeyBinding instance = null;
    [HideInInspector]
    public GameObject firstSelectedButtonInController = null;

    private ControlsSettingsMenu controlsSettingsMenuInstance = null;

    private Dictionary<string, KeyCode> keybindings = new Dictionary<string, KeyCode>();

    [SerializeField] private Image rotatePuzzleArrow;
    [SerializeField] private Image activateButtonInPuzzle;
    [SerializeField] private Image cameraToggle;
    [SerializeField] private Image movementToggle;
    [SerializeField] private Image disableGuard;
    [SerializeField] private Image hackGuard;
    [SerializeField] private Image distactGuardWhileHacking;

    [SerializeField] private GameObject pressAKeyCanvas;
    private GameObject currentSelectedGameObject;
    private EventSystem eventSystem = null;

    private bool canTakeInput = true;
    private bool changedKey = true;
    private Image changingKeyImage;

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


    [SerializeField] Settings settings;
    List<KeyCode> possibleKeyCodes = new List<KeyCode>();

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
        controlsSettingsMenuInstance = ControlsSettingsMenu.instance;
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
                    changingKeyImage.sprite = SetSprite(possibleKeyCodes[i]);
                    changedKey = true;
                    changingKeyImage = null;
                    StartCoroutine("ChangeButtonText");
                }
            }
        }

        UpdateSettings();
    }

    void InitDictionaryKeys()
    {
        //keybindings.Add("RotatePuzzleArrow", KeyCode.JoystickButton3);
        //keybindings.Add("ActivateButtonInPuzzle", KeyCode.JoystickButton1);
        //keybindings.Add("CameraToggle", KeyCode.JoystickButton11);
        //keybindings.Add("MovementToggle", KeyCode.JoystickButton10);
        //keybindings.Add("DisableGuard", KeyCode.JoystickButton1);
        //keybindings.Add("HackGuard", KeyCode.JoystickButton0);
        //keybindings.Add("DistactGuardWhileHacking", KeyCode.JoystickButton1);
        keybindings.Add(settings.rotatePuzzleArrow, settings.rotatePuzzleArrowController);
        keybindings.Add(settings.activateButtonInPuzzle, settings.activateButtonInPuzzleController);
        keybindings.Add(settings.cameraToggle, settings.cameraToggleController);
        keybindings.Add(settings.movementToggle, settings.movementToggleController);
        keybindings.Add(settings.disableGuard, settings.disableGuardController);
        keybindings.Add(settings.hackGuard, settings.hackGuardController);
        keybindings.Add(settings.distractGuardWhileHacking, settings.distractGuardWhileHackingController);
    }

    void SetButtonKeySprite()
    {
        rotatePuzzleArrow.sprite = SetSprite(keybindings["RotatePuzzleArrow"]);
        activateButtonInPuzzle.sprite = SetSprite(keybindings["ActivateButtonInPuzzle"]);
        cameraToggle.sprite = SetSprite(keybindings["CameraToggle"]);
        movementToggle.sprite = SetSprite(keybindings["MovementToggle"]);
        disableGuard.sprite = SetSprite(keybindings["DisableGuard"]);
        hackGuard.sprite = SetSprite(keybindings["HackGuard"]);
        distactGuardWhileHacking.sprite = SetSprite(keybindings["DistactGuardWhileHacking"]);
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
            changingKeyImage = buttonImage;
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
        settings.cameraToggleController = keybindings[settings.cameraToggle];
        settings.movementToggleController = keybindings[settings.movementToggle];
        settings.disableGuardController = keybindings[settings.disableGuard];
        settings.hackGuardController = keybindings[settings.hackGuard];
        settings.distractGuardWhileHackingController = keybindings[settings.distractGuardWhileHacking];
    }
}
