﻿using System.Collections;
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

    [SerializeField] InteractionButtonSubject InteractionButtonSubject = null;
    GameObject circleButton = null;
    GameObject crossButton = null;

    #region Delegates
    public static Action<Transform> createIndicator = delegate { };
    public static Action<Transform> updateIndicator = delegate { };
    public static Action<Transform> removeIndicator = delegate { };
    #endregion

    void Awake()
    {
        circleButton = GameObject.Find("CircleButton");
        circleButton.SetActive(false);
        crossButton = GameObject.Find("CrossButton");
        crossButton.SetActive(false);
        InteractionButtonSubject.AddObserver(this);
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

    void UpdateIndicator(Transform target)
    {
        indicators[target].Register(target, playerVariables.playerTransform, new Action(() => { indicators.Remove(target); }));
    }

    void RemoveIndicator(Transform target)
    {
        if (indicators.ContainsKey(target))
        {
            indicators[target].UnRegister();
            indicators.Remove(target);
        }
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
}
