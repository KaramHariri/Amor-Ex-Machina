using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    //private Dictionary<Transform, SpottedIndicatorTest> indicatorsTest = new Dictionary<Transform, SpottedIndicatorTest>();

    [SerializeField] InteractionButtonSubject InteractionButtonSubject = null;
    GameObject circleButton = null;
    GameObject crossButton = null;

    #region Delegates
    public static Action<Transform> createIndicator = delegate { };
    public static Action<Transform, IndicatorColor> updateIndicator = delegate { };
    public static Action<Transform> removeIndicator = delegate { };
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
    }

    private void OnEnable()
    {
        createIndicator += CreateIndicator;
        removeIndicator += RemoveIndicator;
        updateIndicator += UpdateIndicator;
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

    //void CreateIndicator(Transform target)
    //{
    //    if (!indicatorsTest.ContainsKey(target))
    //    {
    //        SpottedIndicatorTest spottedIndicator = SpottedIndicatorPoolTest.instance.GetIndicator();
    //        indicatorsTest.Add(target, spottedIndicator);
    //    }
    //}

    void UpdateIndicator(Transform target, IndicatorColor indicatorColor)
    {
        indicators[target].Register(target, playerVariables.playerTransform, new Action(() => { indicators.Remove(target); }), indicatorColor);
    }

    //void UpdateIndicator(Transform target)
    //{
    //    indicatorsTest[target].Register(target, playerVariables.playerTransform, new Action(() => { indicatorsTest.Remove(target); }));
    //}

    void RemoveIndicator(Transform target)
    {
        if (indicators.ContainsKey(target))
        {
            indicators[target].UnRegister();
            indicators.Remove(target);
        }
    }

    //void RemoveIndicator(Transform target)
    //{
    //    if (indicatorsTest.ContainsKey(target))
    //    {
    //        indicatorsTest[target].UnRegister();
    //        indicatorsTest.Remove(target);
    //    }
    //}

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
