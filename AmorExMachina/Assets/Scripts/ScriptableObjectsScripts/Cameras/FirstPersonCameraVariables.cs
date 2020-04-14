using UnityEngine;

[CreateAssetMenu(fileName = "FirstPersonCameraVariables", menuName = "Camera/FirstPersonCameraVariables", order = 54)]
public class FirstPersonCameraVariables : ScriptableObject
{
    public Transform followTarget;
    public float verticalSpeed = 150.0f;
    public float horizontalSpeed = 150.0f;
    public float cameraYMin = -70.0f;
    public float cameraYMax = 70.0f;
    public bool invertVerticalInput = false;
    public bool invertHorizontalInput = false;
}
