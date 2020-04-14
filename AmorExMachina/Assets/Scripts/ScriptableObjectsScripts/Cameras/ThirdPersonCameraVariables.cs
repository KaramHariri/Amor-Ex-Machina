using UnityEngine;

[CreateAssetMenu(fileName = "ThirdPersonCameraVariables", menuName = "Camera/ThirdPersonCameraVariables", order = 55)]
public class ThirdPersonCameraVariables : ScriptableObject
{
    public Transform followTarget;
    public float verticalSpeed = 1.5f;
    public float horizontalSpeed = 150.0f;
    public float topRingHeight = 5.0f;
    public float topRingRadius = 4.0f;
    public float middleRingHeight = 2.5f;
    public float middleRingRadius = 7.0f;
    public float bottomRingHeight = -0.7f;
    public float bottomRingRadius = 4.0f;
    public bool invertVerticalInput = false;
    public bool invertHorizontalInput = false;
}
