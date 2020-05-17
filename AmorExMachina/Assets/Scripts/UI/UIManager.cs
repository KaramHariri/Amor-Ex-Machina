using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum InteractionButtons
{
    CIRCLE,
    SQUARE,
    TRIANGLE,
    CROSS
}

public class UIManager : MonoBehaviour, IInteractionButton
{
    [SerializeField] private PlayerVariables playerVariables = null;

    private Dictionary<Transform, SpottedIndicator> indicators = new Dictionary<Transform, SpottedIndicator>();

    [SerializeField] InteractionButtonSubject InteractionButtonSubject = null;

    GameObject circleButton = null;
    GameObject crossButton = null;

    Text hackingTimer = null;
    GameObject hackingSlider = null;
    Image hackingSliderFill = null;


    #region Delegates
    public static Action<Transform> createIndicator = delegate { };
    public static Action<Transform, IndicatorColor> updateIndicator = delegate { };
    public static Action<Transform> removeIndicator = delegate { };

    public static Action activateTimer = delegate { };
    public static Action<float> updateTimer = delegate { };
    public static Action deactivateTimer = delegate { };
    #endregion

    private float changeColorToYellow = 0.0f;
    private float changeColorToRed = 0.0f;

    private bool updatingTimer = false;

    private GlitchEffect glitchEffect = null;
    private AudioManager audioManager = null;

    void Awake()
    {
        InteractionButtonSubject.AddObserver(this);
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Start()
    {
        glitchEffect = Camera.main.GetComponent<GlitchEffect>();

        circleButton = GameObject.Find("CircleButton");
        circleButton.SetActive(false);
        crossButton = GameObject.Find("CrossButton");
        crossButton.SetActive(false);

        hackingTimer = GameObject.Find("HackingTimer").GetComponent<Text>();
        hackingTimer.gameObject.SetActive(false);
        hackingSlider = GameObject.Find("HackingSlider");
        hackingSliderFill = hackingSlider.transform.GetChild(hackingSlider.transform.childCount - 1).GetComponent<Image>();
        hackingSliderFill.fillAmount = 1.0f;
        hackingSlider.SetActive(false);
    }

    private void OnEnable()
    {
        createIndicator += CreateIndicator;
        removeIndicator += RemoveIndicator;
        updateIndicator += UpdateIndicator;

        activateTimer += ActivateTimer;
        updateTimer += UpdateTimer;
        deactivateTimer += DeactivateTimer;
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
        indicators[target].Register(target, playerVariables.playerTransform, new Action(() => { indicators.Remove(target); }), indicatorColor);
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

    void ActivateTimer()
    {
        hackingTimer.text = "0.0";
        hackingTimer.gameObject.SetActive(true);
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
    }

    public void NotifyToShowInteractionButton(InteractionButtons buttonToShow)
    {
        switch (buttonToShow)
        {
            case InteractionButtons.CIRCLE:
                circleButton.SetActive(true);
                break;
            case InteractionButtons.SQUARE:
                break;
            case InteractionButtons.TRIANGLE:
                break;
            case InteractionButtons.CROSS:
                crossButton.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void NotifyToHideInteractionButton(InteractionButtons buttonToHide)
    {
        switch (buttonToHide)
        {
            case InteractionButtons.CIRCLE:
                circleButton.SetActive(false);
                break;
            case InteractionButtons.SQUARE:
                break;
            case InteractionButtons.TRIANGLE:
                break;
            case InteractionButtons.CROSS:
                crossButton.SetActive(false);
                break;
            default:
                break;
        }
    }

    void OnDestroy()
    {
        InteractionButtonSubject.RemoveObserver(this);
        createIndicator -= CreateIndicator;
        removeIndicator -= RemoveIndicator;
        updateIndicator -= UpdateIndicator;

        activateTimer -= ActivateTimer;
        updateTimer -= UpdateTimer;
        deactivateTimer -= DeactivateTimer;
    }
}
