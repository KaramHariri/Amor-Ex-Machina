using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField]
    private PlayerVariables playerVariables;
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void LateUpdate()
    {
        Vector3 newPosition = playerVariables.playerTransform.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;

        //transform.rotation = Quaternion.Euler(90.0f, playerVariables.playerTransform.eulerAngles.y, 0.0f);
        transform.rotation = Quaternion.Euler(90.0f, mainCamera.transform.eulerAngles.y, 0.0f);

    }
}
