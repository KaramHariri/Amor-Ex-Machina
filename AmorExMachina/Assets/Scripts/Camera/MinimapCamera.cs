using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour, IGuardHackedObserver
{
    [SerializeField]
    private PlayerVariables playerVariables = null;
    [SerializeField]
    private PlayerCamerasVariables cameraVariables = null;
    [SerializeField]
    private GuardHackedSubject guardHackedSubject = null;
    private Camera mainCamera = null;
    private Camera minimapCamera = null;
    [SerializeField]
    private float thirdPersonMinimapSize = 10.0f;
    [SerializeField]
    private float firstPersonMinimapSize = 20.0f;
    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float followSpeed = 4.0f;

    Vector3 minimapCameraPosition = Vector3.zero;
    bool switchedToGuardCamera = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        minimapCamera = GetComponent<Camera>();
        guardHackedSubject.AddObserver(this);
        switchedToGuardCamera = false;
    }

    private void Update()
    {
        if (cameraVariables.switchedCameraToFirstPerson || switchedToGuardCamera)
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
        //Vector3 newPosition = playerVariables.playerTransform.position;
        //newPosition.y = transform.position.y;
        //transform.position = newPosition;
        if(switchedToGuardCamera)
        {
            minimapCameraPosition = mainCamera.transform.position;
            minimapCameraPosition.y = transform.position.y;
        }
        else
        {
            minimapCameraPosition = playerVariables.playerTransform.position;
            minimapCameraPosition.y = transform.position.y;
        }
        transform.position = Vector3.Lerp(transform.position, minimapCameraPosition, Time.deltaTime * followSpeed);
        //transform.position = minimapCameraPosition;

        transform.rotation = Quaternion.Euler(90.0f, mainCamera.transform.eulerAngles.y, 0.0f);
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
