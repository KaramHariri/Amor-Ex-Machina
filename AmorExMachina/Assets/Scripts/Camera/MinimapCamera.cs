using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MinimapCamera : MonoBehaviour, IGuardHackedObserver
{
    //private PlayerVariables playerVariables = null;
    private GuardHackedSubject guardHackedSubject = null;
    private Settings settings = null;

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

    private Transform playerTransform = null;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;
        minimapCamera = GetComponent<Camera>();
        switchedToGuardCamera = false;
        firstPersonCamera = false;
    }

    void Start()
    {
        GetStaticReferencesFromGameHandler();
    }

    void OnEnable()
    {
        updateIconSize += UpdateIconSize;
    }

    void OnDisable()
    {
        updateIconSize -= UpdateIconSize;
    }

    void GetStaticReferencesFromGameHandler()
    {
        //playerVariables = GameHandler.playerVariables;
        //if(playerVariables == null)
        //{
        //    Debug.Log("MinimapCamera can't find PlayerVariables in GameHandler");
        //}

        guardHackedSubject = GameHandler.guardHackedSubject;
        if(guardHackedSubject == null)
        {
            Debug.Log("MinimapCamera can't find GuardHackedSubject in GameHandler");
        }
        guardHackedSubject.AddObserver(this);

        settings = GameHandler.settings;
        if (settings == null)
        {
            Debug.Log("MinimapCamera can't find Settings in GameHandler");
        }
    }

    private void Update()
    {
        if (GameHandler.currentState == GameState.NORMALGAME)
        {
            if (Input.GetKeyDown(settings.cameraToggleController) || Input.GetKeyDown(settings.cameraToggleKeyboard))
            {
                firstPersonCamera = !firstPersonCamera;
            }
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
            minimapCameraPosition = playerTransform.position;
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
        if(!firstPersonCamera && !switchedToGuardCamera)
        {
            target.localScale = Vector3.Lerp(target.localScale, Vector3.one * 5.0f, Time.deltaTime * zoomSpeed);
        }
        else
        {
            target.localScale = Vector3.Lerp(target.localScale, Vector3.one * 10.0f, Time.deltaTime * zoomSpeed);
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
