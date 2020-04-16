using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerLastSightPositionObserver
{
    public float walkSpeed = 10.0f;
    public float sneakSpeed = 2.5f;
    public float rotateVelocity = 100.0f;

    Rigidbody rb;
    float verticalInput = 0.0f;
    float horizontalInput = 0.0f;

    Vector3 moveDir = Vector3.zero;
    float moveAmount = 0.0f;
    bool sneaking = false;
    bool controlling = false;
    
    Guard disabledGuard;
    
    public GameStateSubject gameStateSubject;
    public PlayerSoundSubject playerSoundSubject;
    public GuardHackedSubject guardHackedSubject;
    public PlayerVariables playerVariables;
    public BoolVariable cameraSwitchedToFirstPerson;
    public FirstPersonCameraVariables firstPersonCameraVariables;
    public ThirdPersonCameraVariables thirdPersonCameraVariables;
    public TransformVariable firstPersonCameraTransform;
    public TransformVariable thirdPersonCameraTransform;
    public PlayerLastSightPositionSubject PlayerLastSightPositionSubject;

    void Awake()
    {
        playerVariables.playerTransform = transform;
        playerVariables.caught = false;
        rb = GetComponent<Rigidbody>();
        firstPersonCameraVariables.followTarget = transform.GetChild(1);
        thirdPersonCameraVariables.followTarget = transform;
        PlayerLastSightPositionSubject.AddObserver(this);
    }

    void Update()
    {
        if (!playerVariables.caught)
        {
            if (disabledGuard != null && Input.GetButtonDown("Square") && !controlling)
            {
                disabledGuard.beingControlled = true;
                controlling = true;
            }
            else if (disabledGuard != null && Input.GetButtonDown("Square") && controlling)
            {
                disabledGuard.beingControlled = false;
                controlling = false;
            }

            if (controlling)
            {
                guardHackedSubject.GuardHackedNotify(disabledGuard.name);
            }
            else
            {
                guardHackedSubject.GuardHackedNotify("");
            }

            if (playerVariables.caught)
            {
                gameStateSubject.GameStateNotify(GameState.LOST);
            }
            else
            {
                if (controlling == false)
                    GetInput();
            }
            if (controlling == false)
                FPSRotate();
        }
    }

    void FixedUpdate()
    {
        if (!playerVariables.caught)
        {
            if (controlling == false)
            {
                if (!cameraSwitchedToFirstPerson.value && (verticalInput != 0.0f || horizontalInput != 0.0f))
                {
                    HandleRotation();
                }
                HandleMovement();
            }
        }
    }

    void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void FPSRotate()
    {
        if(cameraSwitchedToFirstPerson.value)
        {
            moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

            Quaternion targetRotation = Quaternion.Euler(0.0f, firstPersonCameraTransform.value.eulerAngles.y , 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
        }
    }

    void HandleRotation()
    {
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        Quaternion targetRotation = Quaternion.Euler(0.0f, thirdPersonCameraTransform.value.eulerAngles.y, 0.0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 100.0f * Time.deltaTime);
    }

    void HandleMovement()
    {
        sneaking = Input.GetButton("Sneaking");
        Vector3 v = transform.forward;
        if (cameraSwitchedToFirstPerson.value)
        {
            Vector3 dir = transform.right * horizontalInput + transform.forward * verticalInput;
            v = dir;
        }
        else
        {
            Vector3 dir = thirdPersonCameraTransform.value.transform.right * horizontalInput + thirdPersonCameraTransform.value.forward * verticalInput;
            v = dir;
        }

        v *= ((sneaking) ? sneakSpeed : walkSpeed) * moveAmount;

        v.y = rb.velocity.y;
        rb.velocity = v;

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
                disabledGuard = other.GetComponent<Guard>();
                if (disabledGuard != null)
                {
                    disabledGuard.disabled = true;
                }
                if(Vector3.Distance(disabledGuard.transform.position, transform.position) >= 50.0f)
                {
                    disabledGuard = null;
                }
            }
        }
    }

    public void Notify(Vector3 position)
    {
        if (disabledGuard != null)
        {
            disabledGuard.beingControlled = false;
            disabledGuard = null;
            controlling = false;
        }
    }
}