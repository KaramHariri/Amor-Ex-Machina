using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CharacterController : MonoBehaviour
{
    public float walkSpeed = 5.0f;
    public float sneakSpeed = 2.5f;
    public float rotateVelocity = 100.0f;

    Rigidbody rigidbody;
    float verticalInput = 0.0f;
    float horizontalInput = 0.0f;
    public GameStateSubject gameStateSubject;
    public PlayerSoundSubject playerSoundSubject;
    public PlayerVariables playerVariables;

    public CinemachineFreeLook freeLook;
    public CinemachineVirtualCamera virtualCamera;
    Vector3 moveDir = Vector3.zero;
    float moveAmount = 0.0f;
    [SerializeField]
    bool sneaking = false;
    bool controlling = false;

    public BoolVariable cameraSwitched;

    Guard disabledGuard;

    void Awake()
    {
        playerVariables.playerTransform = transform;
        playerVariables.caught = false;
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (playerVariables.caught)
        {
            gameStateSubject.GameStateNotify(GameState.LOST);
        }
        else
        {
            //if(controlling == false)
            GetInput();
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            disabledGuard.beingControlled = true;
            controlling = true;
        }
        FPSRotate();
    }

    void FixedUpdate()
    {
        if (!cameraSwitched.value && (verticalInput != 0.0f || horizontalInput != 0.0f))
        {
            HandleRotation();
        }
        HandleMovement();
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void FPSRotate()
    {
        if(cameraSwitched.value)
        {
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            Quaternion targetRotation = Quaternion.Euler(0.0f, virtualCamera.transform.eulerAngles.y, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
        }
    }

    void HandleRotation()
    {
        //moveDir = freeLook.transform.forward * verticalInput;
        //moveDir += freeLook.transform.right * horizontalInput;
        //moveDir.Normalize();

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        Quaternion targetRotation = Quaternion.Euler(0.0f, freeLook.transform.eulerAngles.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);

        //Vector3 targetDir = moveDir;

        //targetDir.y = 0;
        //if (targetDir == Vector3.zero)
        //    targetDir = transform.forward;

        //Quaternion tr = Quaternion.LookRotation(targetDir);
        //Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * moveAmount * rotateVelocity);
        //transform.rotation = targetRotation;
    }

    void HandleMovement()
    {
        sneaking = Input.GetButton("Sneaking");
        Vector3 v = transform.forward;
        if (cameraSwitched.value)
        {
            Vector3 dir = transform.right * horizontalInput + transform.forward * verticalInput;
            v = dir;
        }
        else
        {
            Vector3 dir = freeLook.transform.right * horizontalInput + freeLook.transform.forward * verticalInput;
            v = dir;
        }

        v *= ((sneaking) ? sneakSpeed : walkSpeed) * moveAmount;

        v.y = rigidbody.velocity.y;
        rigidbody.velocity = v;

        if (!sneaking)
            playerSoundSubject.NotifyObservers(SoundType.WALKING, transform.position);

    }

    private void OnTriggerStay(Collider other)
    {
        if (other is CapsuleCollider && other.CompareTag("Guard"))
        {
            Vector3 targetToPlayerDirection = transform.position - other.transform.position;
            float angleToTarget = Vector3.Angle(other.transform.forward, targetToPlayerDirection);
            if (angleToTarget < 180.0f && angleToTarget > 135.0f && Input.GetButtonDown("X"))
            {
                Guard guardScript = other.GetComponent<Guard>();
                disabledGuard = other.GetComponent<Guard>();
                if (guardScript != null)
                {
                    guardScript.disabled = true;
                }
            }
        }
    }
}