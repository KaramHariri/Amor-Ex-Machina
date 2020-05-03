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
    }

    void InitDictionaryKeys()
    {
        keybindings.Add("RotatePuzzleArrow", KeyCode.F);
        keybindings.Add("ActivateButtonInPuzzle", KeyCode.E);
        keybindings.Add("CameraToggle", KeyCode.Q);
        keybindings.Add("MovementToggle", KeyCode.LeftShift);
        keybindings.Add("DisableGuard", KeyCode.E);
        keybindings.Add("HackGuard", KeyCode.R);
        keybindings.Add("DistactGuardWhileHacking", KeyCode.E);
    }

    void SetButtonKeyText()
    {
        rotatePuzzleArrow.text = keybindings["RotatePuzzleArrow"].ToString();
        activateButtonInPuzzle.text = keybindings["ActivateButtonInPuzzle"].ToString();
        cameraToggle.text = keybindings["CameraToggle"].ToString();
        movementToggle.text = keybindings["MovementToggle"].ToString();
        disableGuard.text = keybindings["DisableGuard"].ToString();
        hackGuard.text = keybindings["HackGuard"].ToString();
        distactGuardWhileHacking.text = keybindings["DistactGuardWhileHacking"].ToString();
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
        yield return new WaitForSeconds(0.5f);
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
}
