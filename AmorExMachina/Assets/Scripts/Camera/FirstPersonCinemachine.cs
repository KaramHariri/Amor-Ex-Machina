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
    public PlayerCamerasVariables playerCamerasVariables;

    int priority = 20;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.m_Priority = priority;
        cinemachinePOV = cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        mainCamera = Camera.main;
        playerCamerasVariables.firstPersonCameraTransform = this.transform;
        playerCamerasVariables.firstPersonCamera = cinemachineVirtualCamera;
    }

    private void Start()
    {
        SetCameraSettings();
        UpdateCameraSettings();
    }

    private void LateUpdate()
    {
        UpdateCameraSettings();

        if(GameHandler.currentState != GameState.NORMALGAME)
        {
            cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = 0.0f;
            cinemachinePOV.m_VerticalAxis.m_MaxSpeed = 0.0f;
            return;
        }
        RotateCinemachineTransform();
    }

    void RotateCinemachineTransform()
    {
        float playerYAngle = cinemachineVirtualCamera.m_Follow.eulerAngles.y;
        if (playerYAngle > 180)
            playerYAngle -= 360;

        if (!playerCamerasVariables.switchedCameraToFirstPerson)
        {
            cinemachinePOV.m_HorizontalAxis.Value = playerYAngle;
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(0.0f, mainCamera.transform.eulerAngles.y, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
        }
        playerCamerasVariables.firstPersonCameraTransform = this.transform;
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
        cinemachineVirtualCamera.m_Follow = playerCamerasVariables.firstPersonCameraFollowTarget;

        invertVerticalInput = playerCamerasVariables.firstPersonCameraInvertVerticalInput;
        verticalSpeed = playerCamerasVariables.firstPersonCameraVerticalSpeed;
        cameraYMin = playerCamerasVariables.firstPersonCameraYMin;
        cameraYMax = playerCamerasVariables.firstPersonCameraYMax;

        invertHorizontalInput = playerCamerasVariables.firstPersonCameraInvertHorizontalInput;
        horizontalSpeed = playerCamerasVariables.firstPersonCameraHorizontalSpeed;
    }

    void UpdateFirstPersonCameraVariables()
    {
        playerCamerasVariables.firstPersonCameraInvertVerticalInput = invertVerticalInput;
        playerCamerasVariables.firstPersonCameraInvertHorizontalInput = invertHorizontalInput;

        playerCamerasVariables.firstPersonCameraVerticalSpeed = verticalSpeed;
        playerCamerasVariables.firstPersonCameraHorizontalSpeed = horizontalSpeed;

        playerCamerasVariables.firstPersonCameraYMin = cameraYMin;
        playerCamerasVariables.firstPersonCameraYMax = cameraYMax;
    }
}