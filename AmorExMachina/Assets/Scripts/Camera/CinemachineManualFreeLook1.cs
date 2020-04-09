using System.Collections;
using Cinemachine;
using UnityEngine;

public class CinemachineManualFreeLook1 : MonoBehaviour
{
    private CinemachineFreeLook freeLook;
    private CinemachineBrain cinemachineBrain;
    private Camera camera;

    public bool switched = false;

    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        freeLook.m_YAxis.Value = 0.5f;
        camera = Camera.main;
        cinemachineBrain = camera.GetComponent<CinemachineBrain>();
        switched = false;
    }

    private void Update()
    {
        float playerYAngle = freeLook.m_Follow.eulerAngles.y;
        if (playerYAngle > 180)
            playerYAngle -= 360;

        float addPlayerAngle = 90.0f + playerYAngle;
        float subtractPlayerAngle = -90 + playerYAngle;

        if (addPlayerAngle < subtractPlayerAngle)
        {
            freeLook.m_XAxis.m_MinValue = addPlayerAngle;
            freeLook.m_XAxis.m_MaxValue = subtractPlayerAngle;
        }
        else
        {
            freeLook.m_XAxis.m_MinValue = subtractPlayerAngle;
            freeLook.m_XAxis.m_MaxValue = addPlayerAngle;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (freeLook.m_Priority == 22)
            {
                freeLook.m_Priority = 18;
                StartCoroutine("ResetCamera");
            }
            else if (freeLook.m_Priority == 18)
            {
                freeLook.m_Priority = 22;
                switched = false;
                StopCoroutine("ResetCamera");
            }
        }

        if (switched)
        {
            freeLook.m_XAxis.Value = (freeLook.m_XAxis.m_MinValue + freeLook.m_XAxis.m_MaxValue) * 0.5f;
            freeLook.m_YAxis.Value = 0.5f;
        }
    }

    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(cinemachineBrain.m_DefaultBlend.m_Time);
        switched = true;
    }
}