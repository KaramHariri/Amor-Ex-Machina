using Cinemachine;
using UnityEngine;

public class CinemachineManualFreeLook : MonoBehaviour
{
    private CinemachineFreeLook freeLook;

    public float horizontalAimingSpeed = 20f;
    public float verticalAimingSpeed = 20f;

    [Tooltip("This depends on your Free Look rigs setup, use to correct Y sensitivity,"
        + " about 1.5 - 2 results in good Y-X square responsiveness")]
    public float yCorrection = 2f;

    private float xAxisValue;
    private float yAxisValue;
    public float smooth = 0.05f;

    Vector3 targetPos = Vector3.zero;
    Vector3 destination = Vector3.zero;
    Vector3 cameraVelocity = Vector3.zero;

    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * horizontalAimingSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * verticalAimingSpeed * Time.deltaTime;

        freeLook.m_XAxis.ValueRangeLocked = false;
        //freeLook.m_XAxis.m_MaxValue = Vector3.Angle(freeLook.transform.forward, new Vector3(freeLook.m_Follow.right.x - 0.4f, 0.0f, 0.0f));
        //freeLook.m_XAxis.m_MinValue = -Vector3.Angle(freeLook.transform.forward, new Vector3(freeLook.m_Follow.right.x - 0.4f, 0.0f, 0.0f));
        //freeLook.m_XAxis.m_MaxValue = Vector3.Angle(freeLook.transform.position - freeLook.m_Follow.position, new Vector3(freeLook.m_Follow.right.x - 0.4f, 0.0f, 0.0f));
        //freeLook.m_XAxis.m_MinValue = -Vector3.Angle(freeLook.transform.position - freeLook.m_Follow.position, new Vector3(freeLook.m_Follow.right.x - 0.4f, 0.0f, 0.0f));

        //freeLook.m_XAxis.m_MinValue = -Vector3.Angle(freeLook.m_Follow.position - freeLook.transform.position, freeLook.m_Follow.forward);
        //freeLook.m_XAxis.m_MaxValue =  Vector3.Angle(freeLook.m_Follow.position - freeLook.transform.position, freeLook.m_Follow.forward);

        //Debug.Log(Vector3.Angle(freeLook.m_Follow.position - freeLook.transform.position, freeLook.m_Follow.forward));
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (freeLook.m_Priority == 22)
                freeLook.m_Priority = 18;
            else if (freeLook.m_Priority == 18)
                freeLook.m_Priority = 22;
        }

        // Correction for Y
        mouseY /= 360;
        mouseY *= yCorrection;

        xAxisValue += mouseX;
        yAxisValue = Mathf.Clamp01(yAxisValue - mouseY);


        //float angleToTargetforward = Vector3.Angle(freeLook.m_Follow.position - freeLook.transform.position, freeLook.m_Follow.forward);
        //Debug.Log(angleToTargetforward);
        //if (-angleToTargetforward < -70.0f && mouseX < 0.0f)
        //    xAxisValue = -70.0f;
        //if(angleToTargetforward > 70.0f && mouseX > 0.0f)
        //    xAxisValue = 70.0f;
        freeLook.m_XAxis.Value = xAxisValue;
        freeLook.m_YAxis.Value = yAxisValue;
    }
}