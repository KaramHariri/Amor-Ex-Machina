using Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    [System.Serializable]
    public class DebugSettings
    {
        public bool drawDesiredCollisionLines = true;
        public bool drawAdjustedCollisionLines = true;
    }

    private CinemachineFreeLook freeLook;
    public CinemachineCollisionHandler cameraCollisionHandler;

    public float horizontalAimingSpeed = 20f;
    public float verticalAimingSpeed = 20f;

    [Tooltip("This depends on your Free Look rigs setup, use to correct Y sensitivity,"
        + " about 1.5 - 2 results in good Y-X square responsiveness")]
    public float yCorrection = 2f;

    private float xAxisValue;
    private float yAxisValue;

    public float smooth = 0.05f;

    [HideInInspector]
    public float adjustedDistance ;
    public bool smoothFollow = true;

    Vector3 destination = Vector3.zero;
    Vector3 adjustedDestination = Vector3.zero;
    Vector3 cameraVelocity = Vector3.zero;

    public DebugSettings debugSettings = new DebugSettings();

    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        cameraCollisionHandler = GetComponent<CinemachineCollisionHandler>();
        adjustedDistance = freeLook.transform.position.z;
    }

    private void Start()
    {
        cameraCollisionHandler.Initialize(freeLook);
        cameraCollisionHandler.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollisionHandler.adjustedCameraClipPoints);
        cameraCollisionHandler.UpdateCameraClipPoints(destination, transform.rotation, ref cameraCollisionHandler.desiredCameraClipPoints);
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * horizontalAimingSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * verticalAimingSpeed * Time.deltaTime;

        //freeLook.m_XAxis.ValueRangeLocked = false;
        //freeLook.m_XAxis.m_MaxValue = Vector3.Angle(freeLook.transform.forward, freeLook.m_Follow.right);
        //freeLook.m_XAxis.m_MinValue = -Vector3.Angle(freeLook.transform.forward,freeLook.m_Follow.right);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (freeLook.m_Priority == 22)
                freeLook.m_Priority = 18;
            else if (freeLook.m_Priority == 18)
                freeLook.m_Priority = 22;
        }

        // Correction for Y
        mouseY /= 360f;
        mouseY *= yCorrection;

        xAxisValue += mouseX;
        yAxisValue = Mathf.Clamp01(yAxisValue - mouseY);

        freeLook.m_XAxis.Value = xAxisValue;
        freeLook.m_YAxis.Value = yAxisValue;

        cameraCollisionHandler.UpdateCameraClipPoints(transform.position, transform.rotation, ref cameraCollisionHandler.adjustedCameraClipPoints);
        cameraCollisionHandler.UpdateCameraClipPoints(destination, transform.rotation, ref cameraCollisionHandler.desiredCameraClipPoints);

        // draw debug line.
        for (int i = 0; i < 5; i++)
        {
            if (debugSettings.drawDesiredCollisionLines)
            {
                Debug.DrawLine(freeLook.m_Follow.position, cameraCollisionHandler.desiredCameraClipPoints[i], Color.white);
            }
            if (debugSettings.drawAdjustedCollisionLines)
            {
                Debug.DrawLine(freeLook.m_Follow.position, cameraCollisionHandler.adjustedCameraClipPoints[i], Color.green);
            }
        }

        cameraCollisionHandler.CheckColliding(freeLook.m_Follow.position);
        adjustedDistance = cameraCollisionHandler.GetAdjustedDistanceFromTarget(freeLook.m_Follow.position);
    }

    void MoveToTarget()
    {
        if (cameraCollisionHandler.colliding)
        {
            adjustedDestination = Quaternion.Euler(xAxisValue, yAxisValue + freeLook.m_Follow.eulerAngles.y, 0.0f) * Vector3.forward * adjustedDistance;
            adjustedDestination += freeLook.m_Follow.position;
            if (smoothFollow)
            {
                // smooth damp function
                transform.position = Vector3.SmoothDamp(transform.position, adjustedDestination, ref cameraVelocity, smooth);
            }
            else
                transform.position = adjustedDestination;
        }
        else
        {
            if (smoothFollow)
            {
                // smooth damp function.
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref cameraVelocity, smooth);
            }
            else
                transform.position = destination;
        }
    }
}