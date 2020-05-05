using System.Collections;
using Cinemachine;
using UnityEngine;

public class FirstPersonCinemachine : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachinePOV cinemachinePOV;
    private Camera mainCamera;

    //[SerializeField]
    //private float verticalSpeed = 150.0f;
    //[SerializeField]
    //private float horizontalSpeed = 150.0f;
    //[SerializeField]
    //private bool invertVerticalInput = false;
    [SerializeField]
    private bool invertHorizontalInput = false;
    //[SerializeField]
    //private bool useMouseInput = false;
    [SerializeField][Range(-70, 0)]
    private float cameraYMin = -70.0f;
    [SerializeField][Range(0, 70)]
    private float cameraYMax = 70.0f;

    [Header("ScriptableObjects")]
    public PlayerCamerasVariables playerCamerasVariables = null;
    public Settings settings = null;

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
        //cinemachinePOV.m_VerticalAxis.m_InvertInput = invertVerticalInput;
        cinemachinePOV.m_VerticalAxis.m_InvertInput = settings.invertY;
        cinemachinePOV.m_HorizontalAxis.m_InvertInput = invertHorizontalInput;

        //cinemachinePOV.m_VerticalAxis.m_MaxSpeed = verticalSpeed;
        //cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = horizontalSpeed;
        cinemachinePOV.m_VerticalAxis.m_MaxSpeed = settings.firstPersonLookSensitivity;
        cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = settings.firstPersonLookSensitivity;

        cinemachinePOV.m_VerticalAxis.m_MinValue = cameraYMin;
        cinemachinePOV.m_VerticalAxis.m_MaxValue = cameraYMax;

        ControllerConnectedCheck();
        UpdateFirstPersonCameraVariables();
    }

    void SetCameraSettings()
    {
        cinemachineVirtualCamera.m_Follow = playerCamerasVariables.firstPersonCameraFollowTarget;

        //invertVerticalInput = playerCamerasVariables.firstPersonCameraInvertVerticalInput;
        cinemachinePOV.m_VerticalAxis.m_InvertInput = settings.invertY;
        //verticalSpeed = playerCamerasVariables.firstPersonCameraVerticalSpeed;
        cinemachinePOV.m_VerticalAxis.m_MaxSpeed = settings.firstPersonLookSensitivity;
        cameraYMin = playerCamerasVariables.firstPersonCameraYMin;
        cameraYMax = playerCamerasVariables.firstPersonCameraYMax;

        invertHorizontalInput = playerCamerasVariables.firstPersonCameraInvertHorizontalInput;
        //horizontalSpeed = playerCamerasVariables.firstPersonCameraHorizontalSpeed;
        cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = settings.firstPersonLookSensitivity;
    }

    void UpdateFirstPersonCameraVariables()
    {
        //playerCamerasVariables.firstPersonCameraInvertVerticalInput = invertVerticalInput;
        playerCamerasVariables.firstPersonCameraInvertVerticalInput = settings.invertY;
        playerCamerasVariables.firstPersonCameraInvertHorizontalInput = invertHorizontalInput;

        //playerCamerasVariables.firstPersonCameraVerticalSpeed = verticalSpeed;
        //playerCamerasVariables.firstPersonCameraHorizontalSpeed = horizontalSpeed;
        playerCamerasVariables.firstPersonCameraVerticalSpeed = settings.firstPersonLookSensitivity;
        playerCamerasVariables.firstPersonCameraHorizontalSpeed = settings.firstPersonLookSensitivity;

        playerCamerasVariables.firstPersonCameraYMin = cameraYMin;
        playerCamerasVariables.firstPersonCameraYMax = cameraYMax;
    }

    void ControllerConnectedCheck()
    {
        for (int i = 0; i < Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetJoystickNames().Length == 1 && Input.GetJoystickNames()[i] == "")
            {
                cinemachinePOV.m_VerticalAxis.m_InputAxisName = "Mouse Y";
                cinemachinePOV.m_HorizontalAxis.m_InputAxisName = "Mouse X";
            }
            else
            {
                cinemachinePOV.m_VerticalAxis.m_InputAxisName = "CameraVerticalAxis";
                cinemachinePOV.m_HorizontalAxis.m_InputAxisName = "CameraHorizontalAxis";
            }
        }
    }
}