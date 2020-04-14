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
    public ThirdPersonCameraVariables thirdPersonCameraVariables;
    public TransformVariable thirdPersonCameraTransform;
    public PlayerCameras playerCameras;
    public BoolVariable cameraSwitchedToFirstPerson;

    private int priority = 22;

    private void Awake()
    {
        cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        cinemachineFreeLook.m_YAxis.Value = 0.5f;
        cinemachineFreeLook.m_Priority = priority;
        
        //cameraSwitchedToFirstPerson = Resources.Load("ScriptableObjects/Camera/CameraSwitchedToFirstPerson") as BoolVariable;
        //thirdPersonCameraTransform = Resources.Load("ScriptableObjects/Camera/ThirdPersonTransform") as TransformVariable;
        thirdPersonCameraTransform.value = this.transform;
        //playerCameras = Resources.Load("ScriptableObjects/Camera/PlayerCameras") as PlayerCameras;
        playerCameras.thirdPersonCamera = cinemachineFreeLook;
    }

    private void Start()
    {
        //thirdPersonCameraVariables = Resources.Load("ScriptableObjects/Camera/ThirdPersonCameraVariables") as ThirdPersonCameraVariables;
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
        float playerYAngle = cinemachineFreeLook.m_Follow.eulerAngles.y;
        if (playerYAngle > 180)
            playerYAngle -= 360;

        Vector3 targetPos = new Vector3(cinemachineFreeLook.m_Follow.position.x, transform.position.y, cinemachineFreeLook.m_Follow.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);

        if (cameraSwitchedToFirstPerson.value)
        {
            cinemachineFreeLook.m_XAxis.Value = playerYAngle;
            cinemachineFreeLook.m_YAxis.Value = 0.5f;
        }
        thirdPersonCameraTransform.value = this.transform;
    }

    void SetCameraSettings()
    {
        cinemachineFreeLook.m_Follow = thirdPersonCameraVariables.followTarget;
        cinemachineFreeLook.m_LookAt = thirdPersonCameraVariables.followTarget;

        invertVerticalInput = thirdPersonCameraVariables.invertVerticalInput;
        verticalSpeed = thirdPersonCameraVariables.verticalSpeed;

        topRingHeight = thirdPersonCameraVariables.topRingHeight;
        topRingRadius = thirdPersonCameraVariables.topRingRadius;
        middleRingHeight = thirdPersonCameraVariables.middleRingHeight;
        middleRingRadius = thirdPersonCameraVariables.middleRingRadius;
        bottomRingHeight = thirdPersonCameraVariables.bottomRingHeight;
        bottomRingRadius = thirdPersonCameraVariables.bottomRingRadius;

        invertHorizontalInput = thirdPersonCameraVariables.invertHorizontalInput;
        horizontalSpeed = thirdPersonCameraVariables.horizontalSpeed;
    }

    void UpdateThirdPersonCameraVariables()
    {
        thirdPersonCameraVariables.invertVerticalInput = invertVerticalInput;
        thirdPersonCameraVariables.invertHorizontalInput = invertHorizontalInput;

        thirdPersonCameraVariables.topRingHeight = topRingHeight;
        thirdPersonCameraVariables.topRingRadius = topRingRadius;
        thirdPersonCameraVariables.middleRingHeight = middleRingHeight;
        thirdPersonCameraVariables.middleRingRadius = middleRingRadius;
        thirdPersonCameraVariables.bottomRingHeight = bottomRingHeight;
        thirdPersonCameraVariables.bottomRingRadius = bottomRingRadius;

        thirdPersonCameraVariables.verticalSpeed = verticalSpeed;
        thirdPersonCameraVariables.horizontalSpeed = horizontalSpeed;
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