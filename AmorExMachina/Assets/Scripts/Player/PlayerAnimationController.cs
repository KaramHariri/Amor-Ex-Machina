using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private Transform modelTransform;

    private Vector3 inputDirection = new Vector3();

    [SerializeField] private Settings settings = null;
    [SerializeField] private PlayerCamerasVariables cameraVariables = null;

    float verticalInput = 0.0f;
    float horizontalInput = 0.0f;
    bool sneaking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        modelTransform = transform.Find("character_gabriel");
        if (anim == null) { Debug.Log("Can't find the animator"); Debug.Break(); }
        if (modelTransform == null) { Debug.Log("Can't find the model transform"); Debug.Break(); }
    }

    void Update()
    {
        GetInput();

        if (cameraVariables.switchedCameraToFirstPerson)
        {
            inputDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        }
        else
        {
            inputDirection = cameraVariables.thirdPersonCameraTransform.transform.right * horizontalInput + cameraVariables.thirdPersonCameraTransform.forward * verticalInput;
        }

        if (GameHandler.currentState == GameState.NORMALGAME)
        {
            modelTransform.LookAt(transform.position + inputDirection);
        }

        anim.SetFloat("Velocity", inputDirection.magnitude);
        anim.SetBool("Crouching", sneaking);
    }

    private void GetInput()
    {
        if(GameHandler.currentState != GameState.NORMALGAME)
        {
            verticalInput = 0;
            horizontalInput = 0;
        }
        else
        {
            verticalInput = Input.GetAxis("Vertical");
            horizontalInput = Input.GetAxis("Horizontal");
        }

        if (Input.GetKey(settings.movementToggleController) || Input.GetKey(settings.movementToggleKeyboard))
        {
            sneaking = false;
        }
        else
        {
            sneaking = true;
        }
    }
}
