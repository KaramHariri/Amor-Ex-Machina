using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector3 inputDir;

    public float walkSpeed = 5.0f;
    public float sneakSpeed = 2.5f;

    public float turnSmoothTime = 0.2f;
    float turnSmoothVelocity;

    public float speedSmoothTime = 0.1f;
    float speedSmoothVelocity;
    float currentSpeed;

    public PlayerSoundSubject playerSoundSubject;
    public GameStateSubject gameStateSubject;
    public PlayerVariables playerVariables;

    public void Awake()
    {
        playerVariables.playerTransform = transform;
    }

    public void Update()
    {
        if (playerVariables.caught)
        {
            gameStateSubject.GameStateNotify(GameState.LOST);
        }

        if (!playerVariables.caught)
        {
            MovementHandler();
        }
    }

    void MovementHandler()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
        inputDir = input.normalized;
        if (inputDir != Vector3.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        bool sneaking = Input.GetKey(KeyCode.LeftShift);
        float targetSpeed = ((sneaking) ? sneakSpeed : walkSpeed) * inputDir.magnitude;

        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);
        transform.Translate(transform.forward * currentSpeed * Time.deltaTime, Space.World);

        if (input != Vector3.zero && !sneaking)
        {
            playerSoundSubject.NotifyObservers(SoundType.WALKING, transform.position);
        }
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
