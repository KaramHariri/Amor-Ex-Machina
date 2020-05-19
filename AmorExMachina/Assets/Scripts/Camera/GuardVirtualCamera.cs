using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GuardVirtualCamera : MonoBehaviour
{
    private CinemachineVirtualCamera vC;
    private CinemachinePOV cinemachinePOV;
    private Camera mainCamera;

    [SerializeField]
    private bool invertHorizontalInput = false;
    [SerializeField]
    [Range(-70, 0)]
    private float cameraYMin = -70.0f;
    [SerializeField]
    [Range(0, 70)]
    private float cameraYMax = 70.0f;

    private GuardCameraVariables guardCameraVariables;
    private Settings settings = null;

    private void Awake()
    {
        vC = GetComponent<CinemachineVirtualCamera>();
        cinemachinePOV = vC.GetCinemachineComponent<CinemachinePOV>();

        mainCamera = Camera.main;
    }

    private void Start()
    {
        GetStatiReferencesFromGameHandler();
    }

    void GetStatiReferencesFromGameHandler()
    {
        guardCameraVariables = GameHandler.guardCameraVariables;
        if(guardCameraVariables == null)
        {
            Debug.Log("GuardVirtualCamera can't find GuardCameraVariables in GameHandler");
        }

        settings = GameHandler.settings;
        if(settings == null)
        {
            Debug.Log("GuardVirtualCamera can't find Settings in GameHandler");
        }
    }

    private void Update()
    {
        UseControllerInputCheck();
    }

    private void LateUpdate()
    {
        UpdateCameraSettings();
        RotateCinemachineTransform();

        if (vC.m_Priority == 15)
        {
            cinemachinePOV.m_HorizontalAxis.Value = vC.m_Follow.eulerAngles.y;
        }
    }

    void RotateCinemachineTransform()
    {
        Quaternion targetRotation = Quaternion.Euler(0.0f, mainCamera.transform.eulerAngles.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
    }

    void UpdateCameraSettings()
    {
        cinemachinePOV.m_VerticalAxis.m_InvertInput = settings.invertY;
        cinemachinePOV.m_HorizontalAxis.m_InvertInput = invertHorizontalInput;

        cinemachinePOV.m_VerticalAxis.m_MaxSpeed = settings.firstPersonLookSensitivity;
        cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = settings.firstPersonLookSensitivity;

        cinemachinePOV.m_VerticalAxis.m_MinValue = cameraYMin;
        cinemachinePOV.m_VerticalAxis.m_MaxValue = cameraYMax;

        UpdateFirstPersonCameraVariables();
    }

    void UpdateFirstPersonCameraVariables()
    {
        guardCameraVariables.invertVerticalInput = settings.invertY;
        guardCameraVariables.invertHorizontalInput = invertHorizontalInput;

        guardCameraVariables.verticalSpeed = settings.firstPersonLookSensitivity;
        guardCameraVariables.horizontalSpeed = settings.firstPersonLookSensitivity;

        guardCameraVariables.cameraYMin = cameraYMin;
        guardCameraVariables.cameraYMax = cameraYMax;
    }

    void UseControllerInputCheck()
    {
        if (settings.useControllerInput)
        {
            cinemachinePOV.m_VerticalAxis.m_InputAxisName = "CameraVerticalAxis";
            cinemachinePOV.m_HorizontalAxis.m_InputAxisName = "CameraHorizontalAxis";
        }
        else
        {
            cinemachinePOV.m_VerticalAxis.m_InputAxisName = "Mouse Y";
            cinemachinePOV.m_HorizontalAxis.m_InputAxisName = "Mouse X";
        }
    }
}
