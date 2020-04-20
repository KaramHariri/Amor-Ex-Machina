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
    private bool useMouseInput = false;
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
    public PlayerCamerasVariables playerCamerasVariables;

    private int priority = 22;

    private void Awake()
    {
        cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        cinemachineFreeLook.m_YAxis.Value = 0.5f;
        cinemachineFreeLook.m_Priority = priority;
        
        playerCamerasVariables.thirdPersonCameraTransform = this.transform;
        playerCamerasVariables.thirdPersonCamera = cinemachineFreeLook;
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
        playerCamerasVariables.thirdPersonCameraTransform = this.transform;
    }

    void SetCameraSettings()
    {
        cinemachineFreeLook.m_Follow = playerCamerasVariables.thirdPersonCameraFollowTarget;
        cinemachineFreeLook.m_LookAt = playerCamerasVariables.thirdPersonCameraFollowTarget;

        invertVerticalInput = playerCamerasVariables.thirdPersonCameraInvertVerticalInput;
        verticalSpeed = playerCamerasVariables.thirdPersonCameraVerticalSpeed;

        topRingHeight = playerCamerasVariables.thirdPersonCameraTopRingHeight;
        topRingRadius = playerCamerasVariables.thirdPersonCameraTopRingRadius;
        middleRingHeight = playerCamerasVariables.thirdPersonCameraMiddleRingHeight;
        middleRingRadius = playerCamerasVariables.thirdPersonCameraMiddleRingRadius;
        bottomRingHeight = playerCamerasVariables.thirdPersonCameraBottomRingHeight;
        bottomRingRadius = playerCamerasVariables.thirdPersonCameraBottomRingRadius;

        invertHorizontalInput = playerCamerasVariables.thirdPersonCameraInvertHorizontalInput;
        horizontalSpeed = playerCamerasVariables.thirdPersonCameraHorizontalSpeed;
    }

    void UpdateThirdPersonCameraVariables()
    {
        playerCamerasVariables.thirdPersonCameraInvertVerticalInput = invertVerticalInput;
        playerCamerasVariables.thirdPersonCameraInvertHorizontalInput = invertHorizontalInput;

        playerCamerasVariables.thirdPersonCameraTopRingHeight = topRingHeight;
        playerCamerasVariables.thirdPersonCameraTopRingRadius = topRingRadius;
        playerCamerasVariables.thirdPersonCameraMiddleRingHeight = middleRingHeight;
        playerCamerasVariables.thirdPersonCameraMiddleRingRadius = middleRingRadius;
        playerCamerasVariables.thirdPersonCameraBottomRingHeight = bottomRingHeight;
        playerCamerasVariables.thirdPersonCameraBottomRingRadius = bottomRingRadius;

        playerCamerasVariables.thirdPersonCameraVerticalSpeed = verticalSpeed;
        playerCamerasVariables.thirdPersonCameraHorizontalSpeed = horizontalSpeed;
    }

    void UpdateCameraSettings()
    {
        cinemachineFreeLook.m_YAxis.m_InvertInput = invertVerticalInput;
        cinemachineFreeLook.m_XAxis.m_InvertInput = invertHorizontalInput;

        cinemachineFreeLook.m_YAxis.m_MaxSpeed = verticalSpeed;
        cinemachineFreeLook.m_XAxis.m_MaxSpeed = horizontalSpeed;

        cinemachineFreeLook.m_Orbits[0].m_Height = topRingHeight;
        cinemachineFreeLook.m_Orbits[0].m_Radius = topRingRadius;
        cinemachineFreeLook.m_Orbits[1].m_Height = middleRingHeight;
        cinemachineFreeLook.m_Orbits[1].m_Radius = middleRingRadius;
        cinemachineFreeLook.m_Orbits[2].m_Height = bottomRingHeight;
        cinemachineFreeLook.m_Orbits[2].m_Radius = bottomRingRadius;

        if (useMouseInput)
        {
            cinemachineFreeLook.m_YAxis.m_InputAxisName = "Mouse Y";
            cinemachineFreeLook.m_XAxis.m_InputAxisName = "Mouse X";
        }
        else
        {
            cinemachineFreeLook.m_YAxis.m_InputAxisName = "CameraVerticalAxis";
            cinemachineFreeLook.m_XAxis.m_InputAxisName = "CameraHorizontalAxis";
        }

        UpdateThirdPersonCameraVariables();
    }
}