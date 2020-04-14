using System.Collections;
using Cinemachine;
using UnityEngine;

public class FirstPersonCinemachine : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
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
    [SerializeField][Range(-70, 0)]
    private float cameraYMin = -70.0f;
    [SerializeField][Range(0, 70)]
    private float cameraYMax = 70.0f;

    [Header("ScriptableObjects")]
    public FirstPersonCameraVariables firstPersonCameraVariables;
    public TransformVariable firstPersonTransform;
    public PlayerCameras playerCameras;
    public BoolVariable cameraSwitchedToFirstPerson;

    int priority = 20;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.m_Priority = priority;
        cinemachinePOV = cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        mainCamera = Camera.main;
        firstPersonTransform.value = this.transform;
        playerCameras.firstPersonCamera = cinemachineVirtualCamera;
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
    }

    void RotateCinemachineTransform()
    {
        float playerYAngle = cinemachineVirtualCamera.m_Follow.eulerAngles.y;
        if (playerYAngle > 180)
            playerYAngle -= 360;

        if (!cameraSwitchedToFirstPerson.value)
        {
            cinemachinePOV.m_HorizontalAxis.Value = playerYAngle;
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(0.0f, mainCamera.transform.eulerAngles.y, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
        }
        firstPersonTransform.value = this.transform;
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
        cinemachineVirtualCamera.m_Follow = firstPersonCameraVariables.followTarget;

        invertVerticalInput = firstPersonCameraVariables.invertVerticalInput;
        verticalSpeed = firstPersonCameraVariables.verticalSpeed;
        cameraYMin = firstPersonCameraVariables.cameraYMin;
        cameraYMax = firstPersonCameraVariables.cameraYMax;

        invertHorizontalInput = firstPersonCameraVariables.invertHorizontalInput;
        horizontalSpeed = firstPersonCameraVariables.horizontalSpeed;
    }

    void UpdateFirstPersonCameraVariables()
    {
        firstPersonCameraVariables.invertVerticalInput = invertVerticalInput;
        firstPersonCameraVariables.invertHorizontalInput = invertHorizontalInput;
        
        firstPersonCameraVariables.verticalSpeed = verticalSpeed;
        firstPersonCameraVariables.horizontalSpeed = horizontalSpeed;

        firstPersonCameraVariables.cameraYMin = cameraYMin;
        firstPersonCameraVariables.cameraYMax = cameraYMax;
    }
}