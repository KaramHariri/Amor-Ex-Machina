using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardHeadMovement : MonoBehaviour
{
    [SerializeField] private Transform playerNeckTransform;
    private Guard guard;
    private bool updatedRotationAngle;
    private Vector3 idleLookingAroundPositiveVector = Vector3.zero;
    private Vector3 idleLookingAroundNegativeVector = Vector3.zero;

    private void Start()
    {
        playerNeckTransform = GameObject.FindGameObjectWithTag("PlayerNeck").transform;
        guard = GetComponent<Guard>();
    }

    void LateUpdate()
    {
        UpdateLookingAroundAngle();
        if (guard.guardMovement.idle)
        {
            RotateNeckTowards(idleLookingAroundPositiveVector, idleLookingAroundNegativeVector);
        }
        else
        {
            //float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * guard.lookingAroundFrequency));
            guard.guardNeckTransform.localRotation = Quaternion.Lerp(guard.guardNeckTransform.localRotation, Quaternion.Euler(Vector3.zero), Time.deltaTime);
        }
        //if (playerIsInSight)
        //{
        //    Vector3 directionToPlayer = playerTransform.position - headTransform.position;
        //    //directionToPlayer.y = 0;
        //    Quaternion targetQuaternion = Quaternion.LookRotation(directionToPlayer);
        //    headTransform.rotation = Quaternion.Lerp(headTransform.rotation, targetQuaternion, 10f * Time.deltaTime);
        //}
        //else if (guardGotDisabled)
        //{

        //    float guardAngleY = headTransform.eulerAngles.y;
        //    if (guardAngleY > 180.0f)
        //    {
        //        guardAngleY -= 360.0f;
        //    }
        //    else if (guardAngleY < 180.0f)
        //    {
        //        guardAngleY += 360.0f;
        //    }

        //    Quaternion targetQuaternion = Quaternion.Euler(22, guardAngleY, 0);
        //    headTransform.rotation = Quaternion.Lerp(headTransform.rotation, targetQuaternion, 2f * Time.deltaTime);
        //}
    }

    void RotateNeckTowards(Vector3 fromAngle, Vector3 toAngle)
    {
        Quaternion from = Quaternion.Euler(fromAngle);
        Quaternion to = Quaternion.Euler(toAngle);

        float lerp = 0.5F * (1.0F + Mathf.Sin(Mathf.PI * Time.realtimeSinceStartup * guard.lookingAroundFrequency));
        guard.guardNeckTransform.localRotation = Quaternion.Lerp(from, to, lerp);
    }

    public void UpdateLookingAroundAngle()
    {
        if (!updatedRotationAngle)
        {
            idleLookingAroundPositiveVector = new Vector3(0.0f, guard.lookingAroundAngle, 0.0f);
            idleLookingAroundNegativeVector = new Vector3(0.0f, -guard.lookingAroundAngle, 0.0f);
            updatedRotationAngle = true;
        }
    }
}
