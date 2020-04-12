using System.Collections;
using Cinemachine;
using UnityEngine;

public class CinemachineManualFreeLook1 : MonoBehaviour
{
    private CinemachineFreeLook freeLook;
    private CinemachineBrain cinemachineBrain;
    private Camera camera;

    public bool switched = false;
    public BoolVariable cameraSwitched;

    private void Awake()
    {
        freeLook = GetComponent<CinemachineFreeLook>();
        freeLook.m_YAxis.Value = 0.5f;
        camera = Camera.main;
        cinemachineBrain = camera.GetComponent<CinemachineBrain>();
        switched = false;
        cameraSwitched.value = switched;
    }

    private void LateUpdate()
    {
        float playerYAngle = freeLook.m_Follow.eulerAngles.y;
        if (playerYAngle > 180)
            playerYAngle -= 360;

        //float addPlayerAngle = 90.0f + playerYAngle;
        //float subtractPlayerAngle = -90 + playerYAngle;

        //freeLook.m_XAxis.m_MinValue = subtractPlayerAngle;
        //freeLook.m_XAxis.m_MaxValue = addPlayerAngle;

        Vector3 targetPos = new Vector3(freeLook.m_Follow.position.x, transform.position.y, freeLook.m_Follow.position.z);
        Quaternion targetRotation = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);

        bool switching = Input.GetButtonDown("SwitchingCamera");
        if(switching)
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
                cameraSwitched.value = switched;
                StopCoroutine("ResetCamera");
            }
        }

        if (switched)
        {
            freeLook.m_XAxis.Value = playerYAngle;
            freeLook.m_YAxis.Value = 0.5f;
        }
    }

    IEnumerator ResetCamera()
    {
        yield return new WaitForSeconds(cinemachineBrain.m_DefaultBlend.m_Time);
        switched = true;
        cameraSwitched.value = switched;
    }
}