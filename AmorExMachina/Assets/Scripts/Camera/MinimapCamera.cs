using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinimapCamera : MonoBehaviour, IGuardHackedObserver
{
    [SerializeField] private PlayerVariables playerVariables = null;
    [SerializeField] private GuardHackedSubject guardHackedSubject = null;
    [SerializeField] private Settings settings = null;

    private Camera mainCamera = null;
    private Camera minimapCamera = null;

    [SerializeField] private float thirdPersonMinimapSize = 10.0f;
    [SerializeField] private float firstPersonMinimapSize = 20.0f;
    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float followSpeed = 4.0f;

    private Vector3 minimapCameraPosition = Vector3.zero;

    private bool switchedToGuardCamera = false;
    private bool firstPersonCamera = false;

    public static Action<Transform> updateIconSize = delegate { };

    private void Awake()
    {
        mainCamera = Camera.main;
        minimapCamera = GetComponent<Camera>();
        guardHackedSubject.AddObserver(this);
        switchedToGuardCamera = false;
        firstPersonCamera = false;
    }

    void OnEnable()
    {
        updateIconSize += UpdateIconSize;
    }

    void OnDisable()
    {
        updateIconSize -= UpdateIconSize;
    }

    private void Update()
    {
        if( Input.GetKeyDown(settings.cameraToggleController) || Input.GetKeyDown(settings.cameraToggleKeyboard))
        {
            firstPersonCamera = !firstPersonCamera;
        }

        if (firstPersonCamera || switchedToGuardCamera)
        {
            minimapCamera.orthographicSize = Mathf.Lerp(minimapCamera.orthographicSize, firstPersonMinimapSize, Time.deltaTime * zoomSpeed);
        }
        else
        {
            minimapCamera.orthographicSize = Mathf.Lerp(minimapCamera.orthographicSize, thirdPersonMinimapSize, Time.deltaTime * zoomSpeed);
        }
    }

    private void LateUpdate()
    {
        if(!switchedToGuardCamera)
        {
            minimapCameraPosition = playerVariables.playerTransform.position;
            minimapCameraPosition.y = transform.position.y;
        }
        else
        {
            minimapCameraPosition = mainCamera.transform.position;
            minimapCameraPosition.y = transform.position.y;
        }
        transform.position = Vector3.Lerp(transform.position, minimapCameraPosition, Time.deltaTime * followSpeed);

        transform.rotation = Quaternion.Euler(90.0f, mainCamera.transform.eulerAngles.y, 0.0f);
    }

    private void UpdateIconSize(Transform target)
    {
        if(!firstPersonCamera)
        {
            target.localScale = Vector3.Lerp(target.localScale, Vector3.one, Time.deltaTime * zoomSpeed);
        }
        else
        {
            target.localScale = Vector3.Lerp(target.localScale, Vector3.one * 2.0f, Time.deltaTime * zoomSpeed);
        }
    }

    public void GuardHackedNotify(string guardName)
    {
        if (guardName != "")
        {
            switchedToGuardCamera = true;
        }
        else
        {
            switchedToGuardCamera = false;
        }
    }

    void OnDestroy()
    {
        guardHackedSubject.RemoveObserver(this);
    }
    
}
