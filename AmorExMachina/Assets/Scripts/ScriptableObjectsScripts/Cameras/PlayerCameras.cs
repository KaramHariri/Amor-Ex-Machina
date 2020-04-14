using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[CreateAssetMenu(fileName = "PlayerCameras", menuName = "Camera/PlayerCameras", order = 56)]
public class PlayerCameras : ScriptableObject
{
    public CinemachineFreeLook thirdPersonCamera;
    public CinemachineVirtualCamera firstPersonCamera;
}
