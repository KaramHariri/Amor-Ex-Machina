using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public enum InteractionButtons
{
    SQUARE = 0,
    TRIANGLE = 1,
    CROSS = 2,
    R3 = 3,
    R2 = 4,
    OPTIONS = 5,
    R = 6,
    F = 7,
    E = 8,
    Q = 9,
    LEFTSHIFT = 10,
    ESC = 11,
}

public class UIManager : MonoBehaviour, IInteractionButton
{
    private Dictionary<Transform, SpottedIndicator> indicators = new Dictionary<Transform, SpottedIndicator>();

    private InteractionButtonSubject interactionButtonSubject = null;
    private Settings settings = null;

    GameObject[] buttons = null;
    GameObject buttonsCanvas = null;

    TextMeshProUGUI hackingTimer = null;
    GameObject hackingSlider = null;
    GameObject hackingTimerBackground = null;
    Image hackingSliderFill = null;

    GameObject puzzleControlsPanel = null;

    #region Delegates
    public static Action<Transform> createIndicator = delegate { };
    public static Action<Transform, IndicatorColor> updateIndicator = delegate { };
    public static Action<Transform> removeIndicator = delegate { };

    public static Action activateTimer = delegate { };
    public static Action<float> updateTimer = delegate { };
    public static Action deactivateTimer = delegate { };

    public static Action saving = delegate { };

    public static Action activateGameOverPanel = delegate { };

    public static Action activatePuzzleControlsPanel = delegate { };
    public static Action deactivatePuzzleControlsPanel = delegate { };
    #endregion

    private float changeColorToYellow = 0.0f;
    private float changeColorToRed = 0.0f;

    private bool updatingTimer = false;

    private GlitchEffect glitchEffect = null;

    private Transform playerTransform = null;
    private CanvasGroup gameOverCanvasGroup = null;
    private CanvasGroup savingCanvasGroup = null;

    private float savingTimer = 0.0f;
    private float maxSavingTimer = 5.0f;
    private bool startSaving = false;

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        gameOverCanvasGroup = GameObject.Find("GameOverCanvas").GetComponent<CanvasGroup>();
        gameOverCanvasGroup.alpha = 0.0f;
        savingCanvasGroup = GameObject.Find("SavingCanvas").GetComponent<CanvasGroup>();
        savingCanvasGroup.alpha = 0.0f;

        puzzleControlsPanel = GameObject.Find("PuzzleControls");
        puzzleControlsPanel.SetActive(false);
    }

    void Start()
    {
        interactionButtonSubject = GameHandler.interactionButtonSubject;
        if(interactionButtonSubject == null)
        {
            Debug.Log("UIManager can't find InteractionButtonSubject in GameHandler");
        }
        interactionButtonSubject.AddObserver(this);

        settings = GameHandler.settings;
        if (settings == null)
        {
            Debug.Log("UIManager can't find Settings in GameHandler");
        }

        glitchEffect = Camera.main.GetComponent<GlitchEffect>();

        buttonsCanvas = GameObject.Find("ButtonsCanvas");
        buttons = new GameObject[buttonsCanvas.transform.childCount];
        //circleButton = GameObject.Find("CircleButton");
        //circleButton.SetActive(false);
        //crossButton = GameObject.Find("CrossButton");
        //crossButton.SetActive(false);

        InitInteractionButtons();

        hackingTimer = GameObject.Find("HackingTimer").GetComponent<TextMeshProUGUI>();
        if(hackingTimer == null)
        {
            Debug.Log("Can't find TextMeshPro");
        }
        hackingTimer.gameObject.SetActive(false);
        hackingTimerBackground = GameObject.Find("HackingTimerBackground");
        hackingTimerBackground.SetActive(false);
        hackingSlider = GameObject.Find("HackingSlider");
        hackingSliderFill = hackingSlider.transform.GetChild(hackingSlider.transform.childCount - 1).GetComponent<Image>();
        hackingSliderFill.fillAmount = 1.0f;
        hackingSlider.SetActive(false);
    }

    void Update()
    {
        //DeactivateButtons();
    }

    private void OnEnable()
    {
        createIndicator += CreateIndicator;
        removeIndicator += RemoveIndicator;
        updateIndicator += UpdateIndicator;

        activateTimer += ActivateTimer;
        updateTimer += UpdateTimer;
        deactivateTimer += DeactivateTimer;

        activateGameOverPanel += ActivateGameOverPanel;
        saving += Saving;

        activatePuzzleControlsPanel += ActivatePuzzleControlsPanel;
        deactivatePuzzleControlsPanel += DeactivatePuzzleControlsPanel;
    }

    void InitInteractionButtons()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i] = buttonsCanvas.transform.GetChild(i).gameObject;
            buttons[i].SetActive(false);
        }
    }

    void DeactivateButtons()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            if (settings.useControllerInput && i > 5)
                buttons[i].SetActive(false);
            else
                buttons[i].SetActive(false);
        }
    }

    void ActivatePuzzleControlsPanel()
    {
        puzzleControlsPanel.SetActive(true);
    }

    void DeactivatePuzzleControlsPanel()
    {
        puzzleControlsPanel.SetActive(false);
    }

    void Saving()
    {
        startSaving = true;
        StartCoroutine("AnimateSavingText");
    }

    IEnumerator AnimateSavingText()
    {
        while (startSaving)
        {
            savingTimer += Time.deltaTime;
            if (savingTimer < maxSavingTimer)
            {
                savingCanvasGroup.alpha = Mathf.PingPong(Time.time, 1.0f);
            }
            else
            {
                savingCanvasGroup.alpha = Mathf.Lerp(savingCanvasGroup.alpha, 0.0f, Time.deltaTime * 3.0f);
                if (savingCanvasGroup.alpha <= 0.0f)
                {
                    startSaving = false;
                }
            }
            yield return null;
        }
    }

    void CreateIndicator(Transform target)
    {
        if (!indicators.ContainsKey(target))
        {
            //audioManager.Play("GettingDetected");
            SpottedIndicator spottedIndicator = SpottedIndicatorPool.instance.GetIndicator();
            indicators.Add(target, spottedIndicator);
        }
    }

    void UpdateIndicator(Transform target, IndicatorColor indicatorColor)
    {
        indicators[target].Register(target, playerTransform, new Action(() => { indicators.Remove(target); }), indicatorColor);
    }

    void RemoveIndicator(Transform target)
    {
        if (indicators.ContainsKey(target))
        {
            //audioManager.Stop("GettingDetected");
            indicators[target].UnRegister();
            indicators.Remove(target);
        }
    }

    void ActivateGameOverPanel()
    {
        if (gameOverCanvasGroup.alpha < 1.0f)
            gameOverCanvasGroup.alpha += Time.deltaTime;
        else
            gameOverCanvasGroup.alpha = 1.0f;
    }

    void ActivateTimer()
    {
        hackingTimer.text = "0.0";
        hackingTimer.gameObject.SetActive(true);
        hackingTimerBackground.SetActive(true);
        hackingSliderFill.fillAmount = 1.0f;
        hackingSliderFill.color = Color.white;
        changeColorToYellow = hackingSliderFill.fillAmount * 0.75f;
        changeColorToRed = hackingSliderFill.fillAmount * 0.5f;
        glitchEffect.activateGlitchEffect = false;
        updatingTimer = true;
        hackingSlider.SetActive(true);
    }

    void UpdateTimer(float currentTime)
    {
        hackingTimer.text = currentTime.ToString("F1");
        hackingSliderFill.fillAmount = currentTime / 20.0f;
        UpdateTimerColor();
    }

    void UpdateTimerColor()
    {
        if(hackingSliderFill.fillAmount <= changeColorToYellow && hackingSliderFill.fillAmount > changeColorToRed)
        {
            hackingSliderFill.color = Color.Lerp(hackingSliderFill.color, Color.yellow, Time.deltaTime * 0.5f);
        }
        else if(hackingSliderFill.fillAmount <= changeColorToRed)
        {
            hackingSliderFill.color = Color.Lerp(hackingSliderFill.color, Color.red, Time.deltaTime * 0.5f);
            if(updatingTimer)
            {
                if (hackingSliderFill.fillAmount <= (changeColorToRed * 0.25f))
                {
                    glitchEffect.activateGlitchEffect = true;
                    glitchEffect.minDisplacmentAmount = 0.05f;
                    glitchEffect.maxDisplacmentAmount = 0.15f;
                    glitchEffect.glitchUpdateSpeed = UnityEngine.Random.Range(0.05f, 0.1f);
                    glitchEffect.rightStripesAmount = UnityEngine.Random.Range(5.0f, 6.0f);
                    glitchEffect.rightStripesFill = UnityEngine.Random.Range(0.6f, 0.8f);
                    glitchEffect.leftStripesAmount = UnityEngine.Random.Range(5.0f, 6.0f);
                    glitchEffect.leftStripesFill = UnityEngine.Random.Range(0.6f, 0.8f);
                }
                else
                {
                    glitchEffect.activateGlitchEffect = true;
                    glitchEffect.minDisplacmentAmount = 0.03f;
                    glitchEffect.maxDisplacmentAmount = 0.06f;
                    glitchEffect.glitchUpdateSpeed = 0.4f;
                    glitchEffect.rightStripesAmount = UnityEngine.Random.Range(6.3f, 6.5f);
                    glitchEffect.rightStripesFill = UnityEngine.Random.Range(0.75f, 0.8f);
                    glitchEffect.leftStripesAmount = UnityEngine.Random.Range(6.3f, 6.5f);
                    glitchEffect.leftStripesFill = UnityEngine.Random.Range(0.75f, 0.8f);
                }
            }
        }
    }

    void DeactivateTimer()
    {
        hackingTimer.text = "";
        hackingSliderFill.fillAmount = 0.0f;
        changeColorToYellow = hackingSliderFill.fillAmount;
        changeColorToRed = hackingSliderFill.fillAmount;
        glitchEffect.activateGlitchEffect = false;
        updatingTimer = false;
        hackingTimer.gameObject.SetActive(false);
        hackingSlider.SetActive(false);
        hackingTimerBackground.SetActive(false);
    }

    public void NotifyToShowInteractionButton(InteractionButtons buttonToShow)
    {
        InteractionButtons button = buttonToShow;
        int index = 0;
        if (settings.useControllerInput)
        {
            index = (int)button;
            buttons[index + 6].SetActive(false);
        }
        else
        {
            index = (int)button + 6;
            buttons[index - 6].SetActive(false);
        }

        buttons[index].SetActive(true);
    }

    public void NotifyToHideInteractionButton(InteractionButtons buttonToHide)
    {
        InteractionButtons button = buttonToHide;
        int index = 0;
        if (settings.useControllerInput)
        {
            index = (int)button;
            buttons[index + 6].SetActive(false);
        }
        else
        {
            index = (int)button + 6;
            buttons[index - 6].SetActive(false);
        }
        buttons[index].SetActive(false);
    }

    void OnDestroy()
    {
        interactionButtonSubject.RemoveObserver(this);
        createIndicator -= CreateIndicator;
        removeIndicator -= RemoveIndicator;
        updateIndicator -= UpdateIndicator;

        activateTimer -= ActivateTimer;
        updateTimer -= UpdateTimer;
        deactivateTimer -= DeactivateTimer;

        activateGameOverPanel -= ActivateGameOverPanel;
        saving -= Saving;

        activatePuzzleControlsPanel -= ActivatePuzzleControlsPanel;
        deactivatePuzzleControlsPanel -= DeactivatePuzzleControlsPanel;
    }
}
