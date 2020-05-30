using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;
    private PlayerController PC;
    private Transform modelTransform;

    private Vector3 inputDirection = new Vector3();

    //[SerializeField] private Settings settings = null;
    //[SerializeField] private PlayerCamerasVariables cameraVariables = null;

    private Settings settings = null;
    private PlayerCamerasVariables cameraVariables = null;

    float verticalInput = 0.0f;
    float horizontalInput = 0.0f;
    bool sneaking = false;

    private float moveAmount = 0.0f;

    private Transform thirdPersonCameraTransform = null;

    void Start()
    {
        anim = GetComponent<Animator>();
        PC = GetComponent<PlayerController>();
        modelTransform = transform.Find("character_gabriel");
        if (anim == null) { Debug.Log("Can't find the animator"); Debug.Break(); }
        if (modelTransform == null) { Debug.Log("Can't find the model transform"); Debug.Break(); }

        thirdPersonCameraTransform = GameObject.FindGameObjectWithTag("ThirdPersonCamera").transform;

        settings = GameHandler.settings;
        if(settings == null)
        {
            Debug.Log("PlayerAnimationController can't find Settings in GameHandler");
        }

        cameraVariables = GameHandler.playerCamerasVariables;
        if(cameraVariables == null)
        {
            Debug.Log("PlayerAnimationController can't find PlayerCamerasVariables in GameHanlder");
        }
    }

    void Update()
    {
        GetInput();

        //if (cameraVariables.switchedCameraToFirstPerson)
        //{
        //    inputDirection = transform.right * horizontalInput + transform.forward * verticalInput;
        //}
        //else
        //{
            inputDirection = thirdPersonCameraTransform.transform.right * horizontalInput + thirdPersonCameraTransform.forward * verticalInput;
        //}

        if (GameHandler.currentState == GameState.NORMALGAME)
        {
            if (!cameraVariables.switchedCameraToFirstPerson)
                modelTransform.LookAt(transform.position + inputDirection);
            else
            {
                //Debug.Log("In first person view");
                //modelTransform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                //moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
                modelTransform.LookAt(transform.position + FirstPersonCinemachine.firstPersonCameraTransform.forward);
                //Quaternion targetRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
                //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
            }
        }

        // Added 2020-05-29
        inputDirection.Normalize();


        anim.SetFloat("Velocity", inputDirection.magnitude);
        anim.SetBool("Crouching", sneaking);
        anim.SetBool("IsHacking", PC.hacking);
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
