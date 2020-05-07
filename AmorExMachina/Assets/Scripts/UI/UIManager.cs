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

    #region Delegates
    public static Action<Transform> createIndicator = delegate { };
    public static Action<Transform, IndicatorColor> updateIndicator = delegate { };
    public static Action<Transform> removeIndicator = delegate { };

    public static Action activateTimer = delegate { };
    public static Action<float> updateTimer = delegate { };
    public static Action deactivateTimer = delegate { };
    #endregion

    void Awake()
    {
        InteractionButtonSubject.AddObserver(this);
    }

    void Start()
    {
        circleButton = GameObject.Find("CircleButton");
        circleButton.SetActive(false);
        crossButton = GameObject.Find("CrossButton");
        crossButton.SetActive(false);

        hackingTimer = GameObject.Find("HackingTimer").GetComponent<Text>();
        hackingTimer.gameObject.SetActive(false);
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

    private void OnDisable()
    {
        createIndicator -= CreateIndicator;
        removeIndicator -= RemoveIndicator;
        updateIndicator -= UpdateIndicator;
    }

    void CreateIndicator(Transform target)
    {
        if (!indicators.ContainsKey(target))
        {
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
            indicators[target].UnRegister();
            indicators.Remove(target);
        }
    }

    void ActivateTimer()
    {
        hackingTimer.text = "0.0";
        hackingTimer.gameObject.SetActive(true);
    }

    void UpdateTimer(float currentTime)
    {
        hackingTimer.text = currentTime.ToString("F1");
    }

    void DeactivateTimer()
    {
        hackingTimer.text = "";
        hackingTimer.gameObject.SetActive(false);
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
    }
}
