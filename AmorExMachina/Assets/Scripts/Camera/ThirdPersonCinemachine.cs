using System.Collections;
using Cinemachine;
using UnityEngine;

public class ThirdPersonCinemachine : MonoBehaviour
{
    private CinemachineFreeLook cinemachineFreeLook;

    [SerializeField]
    private float verticalSpeed = 1.5f;
    [SerializeField]
    private float horizontalSpeed = 150.0f;
    [SerializeField]
    private bool invertVerticalInput = false;
    [SerializeField]
    private bool invertHorizontalInput = false;
    [SerializeField]
    private float topRingHeight = 5.0f;
    [SerializeField]
    private float topRingRadius = 4.0f;
    [SerializeField]
    private float middleRingHeight = 2.5f;
    [SerializeField]
    private float middleRingRadius = 7.0f;
    [SerializeField]
    private float bottomRingHeight = 0.1f;
    [SerializeField]
    private float bottomRingRadius = 4.0f;

    [Header("ScriptableObjects")]
    private PlayerCamerasVariables playerCamerasVariables = null;
    private Settings settings = null;

    private int priority = 22;

    public static Transform thirdPersonCameraTransform = null;
    public static CinemachineFreeLook thirdPersonCamera = null;

    [SerializeField]
    private bool flipCameraRotation = false;

    private void Awake()
    {
        
        cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        cinemachineFreeLook.m_YAxis.Value = 0.5f;
        cinemachineFreeLook.m_Priority = priority;

        thirdPersonCameraTransform = this.transform;
        thirdPersonCamera = cinemachineFreeLook;

        if (flipCameraRotation)
            cinemachineFreeLook.m_XAxis.Value = 180.0f;
    }

    private void Start()
    {
        playerCamerasVariables = GameHandler.playerCamerasVariables;
        if(playerCamerasVariables == null)
        {
            Debug.Log("ThirdPersonCinemachine can't find PlayerCamerasVariables in GameHandler");
        }

        settings = GameHandler.settings;
        if(settings == null)
        {
            Debug.Log("ThirdPersonCinemachine can't find Settings in GameHandler");
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
            cinemachineFreeLook.m_XAxis.m_MaxSpeed = 0.0f;
            cinemachineFreeLook.m_YAxis.m_MaxSpeed = 0.0f;
            return; 
        }
        RotateCinemachineTransform();
    }

    void RotateCinemachineTransform()
    {
        float playerYAngle = cinemachineFreeLook.m_Follow.eulerAngles.y;
        if (playerYAngle > 180)
            playerYAngle -= 360;

        Vector3 targetPos = new Vector3(cinemachineFreeLook.m_Follow.position.x, transform.position.y, cinemachineFreeLook.m_Follow.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);

        if (playerCamerasVariables.switchedCameraToFirstPerson)
        {
            cinemachineFreeLook.m_XAxis.Value = playerYAngle;
            cinemachineFreeLook.m_YAxis.Value = 0.5f;
        }
        thirdPersonCameraTransform = this.transform;
    }

    void UpdateThirdPersonCameraVariables()
    {
        playerCamerasVariables.thirdPersonCameraInvertVerticalInput = settings.invertY;
        playerCamerasVariables.thirdPersonCameraInvertVerticalInput = settings.invertY;
        playerCamerasVariables.thirdPersonCameraInvertHorizontalInput = invertHorizontalInput;

        playerCamerasVariables.thirdPersonCameraTopRingHeight = topRingHeight;
        playerCamerasVariables.thirdPersonCameraTopRingRadius = topRingRadius;
        playerCamerasVariables.thirdPersonCameraMiddleRingHeight = middleRingHeight;
        playerCamerasVariables.thirdPersonCameraMiddleRingRadius = middleRingRadius;
        playerCamerasVariables.thirdPersonCameraBottomRingHeight = bottomRingHeight;
        playerCamerasVariables.thirdPersonCameraBottomRingRadius = bottomRingRadius;

        playerCamerasVariables.thirdPersonCameraVerticalSpeed = settings.thirdPersonLookSensitivity * 0.01f;
        playerCamerasVariables.thirdPersonCameraHorizontalSpeed = settings.thirdPersonLookSensitivity;
    }

    void UpdateCameraSettings()
    {
        if (settings.useControllerInput)
            cinemachineFreeLook.m_YAxis.m_InvertInput = settings.invertY;
        else
            cinemachineFreeLook.m_YAxis.m_InvertInput = !settings.invertY;

        if (cinemachineFreeLook.m_Follow == null)
        {
            cinemachineFreeLook.m_Follow = PlayerController.thirdPersonCameraAim;
        }
        if (cinemachineFreeLook.m_LookAt == null)
        {
            cinemachineFreeLook.m_LookAt = PlayerController.thirdPersonCameraAim;
        }

        cinemachineFreeLook.m_XAxis.m_InvertInput = invertHorizontalInput;

        cinemachineFreeLook.m_YAxis.m_MaxSpeed = settings.thirdPersonLookSensitivity * 0.01f;
        cinemachineFreeLook.m_XAxis.m_MaxSpeed = settings.thirdPersonLookSensitivity;

        cinemachineFreeLook.m_Orbits[0].m_Height = topRingHeight;
        cinemachineFreeLook.m_Orbits[0].m_Radius = topRingRadius;
        cinemachineFreeLook.m_Orbits[1].m_Height = middleRingHeight;
        cinemachineFreeLook.m_Orbits[1].m_Radius = middleRingRadius;
        cinemachineFreeLook.m_Orbits[2].m_Height = bottomRingHeight;
        cinemachineFreeLook.m_Orbits[2].m_Radius = bottomRingRadius;

        UpdateThirdPersonCameraVariables();
    }

    void UseControllerInputCheck()
    {
        if(settings.useControllerInput)
        {
            cinemachineFreeLook.m_YAxis.m_InputAxisName = "CameraVerticalAxis";
            cinemachineFreeLook.m_XAxis.m_InputAxisName = "CameraHorizontalAxis";
        }
        else
        {
            cinemachineFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";
            cinemachineFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
        }
    }
}