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
    private float verticalSpeed = 150.0f;
    [SerializeField]
    private float horizontalSpeed = 150.0f;
    [SerializeField]
    private bool invertVerticalInput = false;
    [SerializeField]
    private bool invertHorizontalInput = false;
    [SerializeField]
    private bool useMouseInput = false;
    [SerializeField]
    [Range(-70, 0)]
    private float cameraYMin = -70.0f;
    [SerializeField]
    [Range(0, 70)]
    private float cameraYMax = 70.0f;
    public GuardCameraVariables guardCameraVariables;

    private Transform follow;

    private void Awake()
    {
        vC = GetComponent<CinemachineVirtualCamera>();
        cinemachinePOV = vC.GetCinemachineComponent<CinemachinePOV>();

        //follow = GameObject.Find(transform.name).transform.GetChild(1);
        //vC.m_Follow = follow;
        mainCamera = Camera.main;
        //vC.m_Follow = transform.parent;
    }

    private void Start()
    {
        SetCameraSettings();
        UpdateCameraSettings();
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
        cinemachinePOV.m_VerticalAxis.m_InvertInput = invertVerticalInput;
        cinemachinePOV.m_HorizontalAxis.m_InvertInput = invertHorizontalInput;

        cinemachinePOV.m_VerticalAxis.m_MaxSpeed = verticalSpeed;
        cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = horizontalSpeed;

        cinemachinePOV.m_VerticalAxis.m_MinValue = cameraYMin;
        cinemachinePOV.m_VerticalAxis.m_MaxValue = cameraYMax;

        if (useMouseInput)
        {
            cinemachinePOV.m_VerticalAxis.m_InputAxisName = "Mouse Y";
            cinemachinePOV.m_HorizontalAxis.m_InputAxisName = "Mouse X";
        }
        else
        {
            cinemachinePOV.m_VerticalAxis.m_InputAxisName = "CameraVerticalAxis";
            cinemachinePOV.m_HorizontalAxis.m_InputAxisName = "CameraHorizontalAxis";
        }

        UpdateFirstPersonCameraVariables();
    }

    void SetCameraSettings()
    {
        //vC.m_Follow = firstPersonCameraVariables.followTarget;

        invertVerticalInput = guardCameraVariables.invertVerticalInput;
        verticalSpeed = guardCameraVariables.verticalSpeed;
        cameraYMin = guardCameraVariables.cameraYMin;
        cameraYMax = guardCameraVariables.cameraYMax;

        invertHorizontalInput = guardCameraVariables.invertHorizontalInput;
        horizontalSpeed = guardCameraVariables.horizontalSpeed;
    }

    void UpdateFirstPersonCameraVariables()
    {
        guardCameraVariables.invertVerticalInput = invertVerticalInput;
        guardCameraVariables.invertHorizontalInput = invertHorizontalInput;

        guardCameraVariables.verticalSpeed = verticalSpeed;
        guardCameraVariables.horizontalSpeed = horizontalSpeed;

        guardCameraVariables.cameraYMin = cameraYMin;
        guardCameraVariables.cameraYMax = cameraYMax;
    }
}
