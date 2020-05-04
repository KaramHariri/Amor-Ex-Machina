using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "PlayerCamerasVariables", menuName = "Camera/PlayerCamerasVariables", order = 56)]
public class PlayerCamerasVariables : ScriptableObject
{
    [Header("SwitchingCamera")]
    public bool switchedCameraToFirstPerson;

    [Header("Third Person Camera")]
    public CinemachineFreeLook thirdPersonCamera;
    public Transform thirdPersonCameraTransform;
    public Transform thirdPersonCameraFollowTarget;
    public float thirdPersonCameraVerticalSpeed = 1.5f;
    public float thirdPersonCameraHorizontalSpeed = 150.0f;
    public float thirdPersonCameraTopRingHeight = 5.0f;
    public float thirdPersonCameraTopRingRadius = 4.0f;
    public float thirdPersonCameraMiddleRingHeight = 2.5f;
    public float thirdPersonCameraMiddleRingRadius = 7.0f;
    public float thirdPersonCameraBottomRingHeight = -0.7f;
    public float thirdPersonCameraBottomRingRadius = 4.0f;
    public bool thirdPersonCameraInvertVerticalInput = false;
    public bool thirdPersonCameraInvertHorizontalInput = false;

    [Header("First Person Camera")]
    public CinemachineVirtualCamera firstPersonCamera;
    public Transform firstPersonCameraTransform;
    public Transform firstPersonCameraFollowTarget;
    public float firstPersonCameraVerticalSpeed = 150.0f;
    public float firstPersonCameraHorizontalSpeed = 150.0f;
    public float firstPersonCameraYMin = -70.0f;
    public float firstPersonCameraYMax = 70.0f;
    public bool firstPersonCameraInvertVerticalInput = false;
    public bool firstPersonCameraInvertHorizontalInput = false;

}
