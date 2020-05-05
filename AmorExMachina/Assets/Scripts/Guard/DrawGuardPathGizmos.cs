using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGuardPathGizmos : MonoBehaviour
{
    [SerializeField] private bool drawWayPointGizmos = false;

    private void OnDrawGizmos()
    {
        if (drawWayPointGizmos)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform currentPathHolder = transform.GetChild(i);
                Vector3 startPosition = currentPathHolder.GetChild(0).position;
                Vector3 previousPosition = startPosition;

                foreach (Transform wayPoint in currentPathHolder)
                {
                    Gizmos.DrawSphere(wayPoint.position, 0.3f);
                    Gizmos.DrawLine(previousPosition, wayPoint.position);
                    previousPosition = wayPoint.position;
                }
                Gizmos.DrawLine(previousPosition, startPosition);
            }
        }
    }
}
