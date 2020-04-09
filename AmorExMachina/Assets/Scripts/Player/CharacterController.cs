using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float inputDelay = 0.1f;
    public float walkSpeed = 5.0f;
    public float sneakSpeed = 2.5f;
    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;
    public float rotateVelocity = 100.0f;

    Quaternion targetRotation;
    Vector3 velocity = Vector3.zero;
    Rigidbody rigidbody;
    float forwardInput = 0.0f;
    float turnInput = 0.0f;
    public GameStateSubject gameStateSubject;
    public PlayerSoundSubject playerSoundSubject;
    public PlayerVariables playerVariables;

    void Awake()
    {
        playerVariables.playerTransform = transform;
        playerVariables.caught = false;
    }
    void Start()
    {
        targetRotation = transform.rotation;
        rigidbody = GetComponent<Rigidbody>();
    }

    void GetInput()
    {
        forwardInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void Update()
    {
        if (playerVariables.caught)
        {
            gameStateSubject.GameStateNotify(GameState.LOST);
        }
        else
        {
            GetInput();
            Turn();
        }
    }

    void FixedUpdate()
    {
        Move();
        rigidbody.velocity = transform.TransformDirection(velocity);
    }

    void Move()
    {
        bool sneaking = Input.GetKey(KeyCode.LeftShift);
        if (Mathf.Abs(forwardInput) > inputDelay)
        {
            velocity.z = ((sneaking) ? sneakSpeed : walkSpeed) * forwardInput;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, velocity.z, ref speedSmoothVelocity, speedSmoothTime);
            if(!sneaking)
                playerSoundSubject.NotifyObservers(SoundType.WALKING, transform.position);
        }
        else
            velocity.z = 0.0f;
    }

    void Turn()
    {
        if (Mathf.Abs(turnInput) > inputDelay)
        {
            targetRotation *= Quaternion.AngleAxis(rotateVelocity * turnInput * Time.deltaTime, Vector3.up);
        }
        transform.rotation = targetRotation;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other is CapsuleCollider && other.CompareTag("Guard"))
        {
            Vector3 targetToPlayerDirection = transform.position - other.transform.position;
            float angleToTarget = Vector3.Angle(other.transform.forward, targetToPlayerDirection);
            if (angleToTarget < 180.0f && angleToTarget > 135.0f && Input.GetKeyDown(KeyCode.Space))
            {
                Guard guardScript = other.GetComponent<Guard>();
                if (guardScript != null)
                {
                    guardScript.disabled = true;
                }
            }
        }
    }
}