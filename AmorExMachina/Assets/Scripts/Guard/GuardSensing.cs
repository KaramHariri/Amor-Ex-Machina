﻿using System.Collections.Generic;
using UnityEngine;

public class GuardSensing : MonoBehaviour
{
    public bool playerInSight = false;
    public bool canHear = false;
    public bool suspicious = false;

    [HideInInspector]
    public UnityEngine.AI.NavMeshAgent navMeshAgent;
    [HideInInspector]
    public SphereCollider sensingCollider;

    [HideInInspector]
    public List<Guard> disabledGuards;

    GuardVariables guardVariables;
    PlayerVariables playerVariables;

    public void GuardSensingAwake()
    {
        sensingCollider = GetComponent<SphereCollider>();
        navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public void Update()
    {
        sensingCollider.radius = guardVariables.fieldOfViewRadius;
    }

    public void SetScriptablesObjects(GuardVariables guardVariablesScriptableObject, PlayerVariables playerVariablesScriptableObject)
    {
        guardVariables = guardVariablesScriptableObject;
        playerVariables = playerVariablesScriptableObject;
    }

    public bool CheckPlayerInSight()
    {
        if (playerInSight)
            return true;

        return false;
    }

    public bool Suspicious()
    {
        if (suspicious)
        {
            return true;
        }
        return false;
    }

    public bool FoundKnockedOutGuard()
    {
        if (disabledGuards.Count > 0)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerVariables.playerTransform.gameObject)
        {
            canHear = true;

            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < guardVariables.fieldOfViewAngle)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(transform.position, direction.normalized, out raycastHit, sensingCollider.radius))
                {
                    if (raycastHit.collider.gameObject == playerVariables.playerTransform.gameObject)
                    {
                        playerInSight = true;
                    }
                    else
                        playerInSight = false;
                }
            }
        }

        if (other.gameObject.CompareTag("Guard") && other.gameObject != this.gameObject)
        {
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            if (angle < guardVariables.fieldOfViewAngle)
            {
                if (other.GetComponent<Guard>().disabled)
                {
                    RaycastHit raycastHit;
                    if (Physics.Raycast(transform.position, direction.normalized, out raycastHit, sensingCollider.radius))
                    {
                        if (raycastHit.collider.gameObject == other.gameObject)
                        {
                            if (!disabledGuards.Contains(other.GetComponent<Guard>()))
                                disabledGuards.Add(other.GetComponent<Guard>());
                        }
                    }
                }
                else
                {
                    if (disabledGuards.Contains(other.GetComponent<Guard>()))
                        disabledGuards.Remove(other.GetComponent<Guard>());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerVariables.playerTransform.gameObject)
        {
            playerInSight = false;
            canHear = false;
        }
    }

    public float CalculateLength(Vector3 targetPosition)
    {
        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
        if (navMeshAgent.enabled)
            navMeshAgent.CalculatePath(targetPosition, path);
        Vector3[] allWayPoints = new Vector3[path.corners.Length + 2];

        allWayPoints[0] = transform.position;
        allWayPoints[allWayPoints.Length - 1] = targetPosition;


        for (int i = 0; i < path.corners.Length; i++)
        {
            allWayPoints[i + 1] = path.corners[i];
        }

        float pathLength = 0.0f;
        for (int i = 0; i < allWayPoints.Length - 1; i++)
        {
            pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
        }

        return pathLength;
    }

    public void Reset()
    {
        playerInSight = false;
        canHear = false;
        suspicious = false;
    }
}
