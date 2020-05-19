using System.Collections;
using Cinemachine;
using UnityEngine;

public class FirstPersonCinemachine : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachinePOV cinemachinePOV;
    private Camera mainCamera;

    [SerializeField]
    private bool invertHorizontalInput = false;
    [SerializeField]
    private bool useMouseInput = false;
    [SerializeField][Range(-70, 0)]
    private float cameraYMin = -70.0f;
    [SerializeField][Range(0, 70)]
    private float cameraYMax = 70.0f;

    private PlayerCamerasVariables playerCamerasVariables = null;
    private Settings settings = null;

    int priority = 20;

    public static Transform firstPersonCameraTransform = null;
    public static CinemachineVirtualCamera firstPersonCamera = null;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.m_Priority = priority;
        cinemachinePOV = cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        mainCamera = Camera.main;
        firstPersonCameraTransform = this.transform;
        firstPersonCamera = cinemachineVirtualCamera;
    }

    private void Start()
    {
        playerCamerasVariables = GameHandler.playerCamerasVariables;
        if(playerCamerasVariables == null)
        {
            Debug.Log("FirstPersonCinemachine can't find PlayerCamerasVariables in GameHandler");
        }

        settings = GameHandler.settings;
        if(settings == null)
        {
            Debug.Log("FirstPersonCinemachine can't find Settings in GameHandler");
        }
    }

    private void Update()
    {
        UseControllerInputCheck();
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

        firstPersonCameraTransform = this.transform;
        firstPersonCamera = cinemachineVirtualCamera;
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

    void SetCameraSettings()
    {
        cinemachineVirtualCamera.m_Follow = PlayerController.firstPersonCameraAim;

        cinemachinePOV.m_VerticalAxis.m_InvertInput = settings.invertY;
        cinemachinePOV.m_VerticalAxis.m_MaxSpeed = settings.firstPersonLookSensitivity;
        cameraYMin = playerCamerasVariables.firstPersonCameraYMin;
        cameraYMax = playerCamerasVariables.firstPersonCameraYMax;

        invertHorizontalInput = playerCamerasVariables.firstPersonCameraInvertHorizontalInput;
        cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = settings.firstPersonLookSensitivity;
    }

    void UpdateFirstPersonCameraVariables()
    {
        cinemachineVirtualCamera.m_Follow = PlayerController.firstPersonCameraAim;

        playerCamerasVariables.firstPersonCameraInvertVerticalInput = settings.invertY;
        playerCamerasVariables.firstPersonCameraInvertHorizontalInput = invertHorizontalInput;

        playerCamerasVariables.firstPersonCameraVerticalSpeed = settings.firstPersonLookSensitivity;
        playerCamerasVariables.firstPersonCameraHorizontalSpeed = settings.firstPersonLookSensitivity;

        playerCamerasVariables.firstPersonCameraYMin = cameraYMin;
        playerCamerasVariables.firstPersonCameraYMax = cameraYMax;
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