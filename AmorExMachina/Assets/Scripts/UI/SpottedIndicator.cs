using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum IndicatorColor
{
    Red,
    Yellow
}

public class SpottedIndicator : MonoBehaviour
{
    private RectTransform rect = null;
    protected RectTransform Rect
    {
        get
        {
            if (rect == null)
            {
                rect = GetComponent<RectTransform>();
                if (rect == null)
                {
                    rect = gameObject.AddComponent<RectTransform>();
                }
            }
            return rect;
        }
    }

    public Transform target { get; protected set; } = null;
    private Transform player = null;

    private Action unRegister = null;

    private Quaternion tRot = Quaternion.identity;
    private Vector3 tPos = Vector3.zero;

    [SerializeField] private Image image = null;
    private GuardSensing guardSensing = null;

    private AudioManager audioManager = null;
    private bool detectionSoundPlayed = false;

    private void Awake()
    {
        image.color = new Color(1.0f, 0.92f, 0.016f, 0.8f);
        audioManager = FindObjectOfType<AudioManager>();
        image.fillAmount = 0.0f;
    }

    public void Register(Transform t, Transform p, Action unRegister, IndicatorColor indicatorColor)
    {
        this.target = t;
        this.player = p;
        this.unRegister = unRegister;

        if (!gameObject.activeInHierarchy)
        {
            guardSensing = t.GetComponent<GuardSensing>();
            gameObject.SetActive(true);
        }

        if (indicatorColor == IndicatorColor.Red)
        {
            if(!detectionSoundPlayed)
            {
                audioManager.Play("GettingDetected");
                detectionSoundPlayed = true;
            }
            image.color = Color.Lerp(image.color, new Color(1.0f, 0.0f, 0.0f, 0.8f), Time.deltaTime * 2.0f);
        }
        else
            image.color = Color.Lerp(image.color, new Color(1.0f, 0.92f, 0.016f, 0.8f), Time.deltaTime);

        image.fillAmount = guardSensing.detectionAmount / guardSensing.maxDetectionAmount;

        RotateToTheTarget();
    }

    public void UnRegister()
    {
        unRegister();
        if (gameObject.activeInHierarchy)
        {
            detectionSoundPlayed = false;
            gameObject.SetActive(false);
        }
    }

    void RotateToTheTarget()
    {
        if (target != null)
        {
            tPos = target.position;
            tRot = target.rotation;
        }
        Vector3 direction = player.position - tPos;
        direction = direction.normalized;

        tRot = Quaternion.LookRotation(direction);
        tRot.z = -tRot.y;
        tRot.x = 0.0f;
        tRot.y = 0.0f;

        Vector3 northDirection = new Vector3(0, 0, player.eulerAngles.y);
        Rect.localRotation = tRot * Quaternion.Euler(northDirection);
    }
}
