using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SpottedIndicatorTest : MonoBehaviour
{
    private CanvasGroup canvasGroup = null;
    protected CanvasGroup CanvasGroup
    {
        get
        {
            if(canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if(canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
            return canvasGroup;
        }
    }

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

    [SerializeField]
    private Image image;

    private void Awake()
    {
        image.fillAmount = 0.0f;
    }

    public void Register(Transform t, Transform p, Action unRegister)
    {
        this.target = t;
        this.player = p;
        this.unRegister = unRegister;

        if (!gameObject.activeInHierarchy)
            gameObject.SetActive(true);

        image.fillAmount = t.GetComponent<GuardSensing>().detectionAmount / t.GetComponent<GuardSensing>().maxDetectionAmount;

        RotateToTheTarget();
        if(CanvasGroup.alpha < 1.0f)
        {
            CanvasGroup.alpha += Time.deltaTime;
        }
    }

    public void UnRegister()
    {
        unRegister();
        //Destroy(gameObject);
        if(gameObject.activeInHierarchy)
            gameObject.SetActive(false);
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
