using System.Collections;
using Cinemachine;
using UnityEngine;

public class CinemachineFPS : MonoBehaviour
{
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachinePOV cinemachinePOV;
    Camera camera;

    public BoolVariable cameraSwitched;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachinePOV = cinemachineVirtualCamera.GetCinemachineComponent<CinemachinePOV>();
        camera = Camera.main;
    }

    private void LateUpdate()
    {
        float playerYAngle = cinemachineVirtualCamera.m_Follow.eulerAngles.y;
        if (playerYAngle > 180)
            playerYAngle -= 360;

        if (!cameraSwitched.value)
        {
            cinemachinePOV.m_HorizontalAxis.Value = playerYAngle;
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(0.0f, camera.transform.eulerAngles.y, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
        }
    }
}