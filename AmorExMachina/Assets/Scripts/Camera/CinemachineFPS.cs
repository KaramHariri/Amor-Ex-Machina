//using System.Collections;
//using Cinemachine;
//using UnityEngine;

//public class CinemachineFPS : MonoBehaviour
//{
//    private CinemachineVirtualCamera cinemachineVirtualCamera;

//    private void Awake()
//    {
//        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
//    }

//    private void Update()
//    {

//        float playerYAngle = cinemachineVirtualCamera.m_Follow.eulerAngles.y;
//        if (playerYAngle > 180)
//            playerYAngle -= 360;

//        float addPlayerAngle = 90.0f + playerYAngle;
//        float subtractPlayerAngle = -90 + playerYAngle;

//        if (addPlayerAngle < subtractPlayerAngle)
//        {
//            cinemachineVirtualCamera.
//            cinemachineVirtualCamera.m_XAxis.m_MinValue = addPlayerAngle;
//            freeLook.m_XAxis.m_MaxValue = subtractPlayerAngle;
//        }
//        else
//        {
//            freeLook.m_XAxis.m_MinValue = subtractPlayerAngle;
//            freeLook.m_XAxis.m_MaxValue = addPlayerAngle;
//        }

//        if (Input.GetKeyDown(KeyCode.E))
//        {
//            if (freeLook.m_Priority == 22)
//            {
//                freeLook.m_Priority = 18;
//                StartCoroutine("ResetCamera");
//            }
//            else if (freeLook.m_Priority == 18)
//            {
//                freeLook.m_Priority = 22;
//                switched = false;
//                StopCoroutine("ResetCamera");
//            }
//        }

//        if (switched)
//        {
//            freeLook.m_XAxis.Value = (freeLook.m_XAxis.m_MinValue + freeLook.m_XAxis.m_MaxValue) * 0.5f;
//            freeLook.m_YAxis.Value = 0.5f;
//            xAxisValue = freeLook.m_XAxis.Value;
//            yAxisValue = freeLook.m_YAxis.Value;
//        }
//        else
//        { 
//            // Correction for Y
//            mouseY /= 360;
//            mouseY *= yCorrection;

//            xAxisValue += mouseX;
//            yAxisValue = Mathf.Clamp01(yAxisValue - mouseY);

//            freeLook.m_XAxis.Value = xAxisValue;
//            freeLook.m_YAxis.Value = yAxisValue;
//        }
//    }

//    IEnumerator ResetCamera()
//    {
//        yield return new WaitForSeconds(cinemachineBrain.m_DefaultBlend.m_Time);
//        switched = true;
//    }
//}