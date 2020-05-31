using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleActivator : MonoBehaviour, IPlayerSpottedObserver
{
    public Vector3 offScreenPosition;
    public Vector3 onScreenPosition;
    public GameObject objectToMove;
    public AnimationCurve animationCurve;
    [HideInInspector]
    public bool activated = false;
    [HideInInspector] public bool puzzleSolved = false;

    public float duration = 1.5f;
    public float animationCooldown = 0f;
    private float deactivationDelay = 0.2f;

    private bool canBeActivated = false;

    private AudioManager audioManager;
    private PlayerSpottedSubject playerSpottedSubject = null;
    private InteractionButtonSubject interactionButtonSubject = null;
    private Settings settings = null;

    private Transform player = null;
    private Transform playerTransform = null;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        player = playerTransform.Find("character_gabriel");
    }

    private void Start()
    {
        GetStaticReferencesFromGameHandler();
    }

    void GetStaticReferencesFromGameHandler()
    {
        audioManager = GameHandler.audioManager;
        if (audioManager == null)
        {
            Debug.Log("PuzzleActivator can't find AudioManager in GameHandler");
        }

        playerSpottedSubject = GameHandler.playerSpottedSubject;
        if (playerSpottedSubject == null)
        {
            Debug.Log("PuzzleActivator can't find PlayerSpottedSubject in GameHandler");
        }
        playerSpottedSubject.AddObserver(this);

        interactionButtonSubject = GameHandler.interactionButtonSubject;
        if (interactionButtonSubject == null)
        {
            Debug.Log("PuzzleActivator can't find InteractionButtonSubject in GameHandler");
        }

        settings = GameHandler.settings;
        if (settings == null)
        {
            Debug.Log("PuzzleActivator can't find Settings in GameHandler");
        }
    }

    public void Update()
    {
        if(puzzleSolved) { return; }

        animationCooldown = animationCooldown - Time.deltaTime;
        if (animationCooldown < 0)
            animationCooldown = 0;

        if (!canBeActivated) { return; }

        Vector3 directionToLockFromPlayer = transform.position - playerTransform.position;
        directionToLockFromPlayer.y = 0;
        directionToLockFromPlayer.Normalize();
        Vector3 playerForwardDirection = player.transform.forward;
        playerForwardDirection.y = 0;
        //Debug.Log("DirToLock" + directionToLockFromPlayer);
        //Debug.DrawRay(playerTransform.position, directionToLockFromPlayer, Color.red);
        //Debug.DrawRay(playerTransform.position, playerForwardDirection, Color.blue);
        //Debug.Log(Vector3.Angle(directionToLockFromPlayer, playerForwardDirection));

        interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.SQUARE);

        if (Vector3.Angle(directionToLockFromPlayer, playerForwardDirection) > 75) { return; }

        if(!activated)
            interactionButtonSubject.NotifyToShowInteractionButton(InteractionButtons.SQUARE);
        //if (Input.GetButtonDown("Circle") && animationCooldown <= 0)
        if ((Input.GetKeyDown(settings.activatePuzzleController) || Input.GetKeyDown(settings.activatePuzzleKeyboard)) && animationCooldown <= 0)
        {
            //if(angleToPlayer > 180.0f || angleToPlayer < 90.0f) { return; }
            if (!activated)
            {
                audioManager.Play("ActivateDoorPuzzle");
                //Debug.Log("Activated");
                MoveTo(offScreenPosition, onScreenPosition, duration);
                animationCooldown = duration + 0.2f;
                activated = !activated;
                GameHandler.currentState = GameState.PUZZLE;

                // Added 20-05-28
                UIManager.activatePuzzleControlsPanel();
                ////
            }
            else
            {
                audioManager.Play("ActivateDoorPuzzle");
                //Debug.Log("Deactivated");
                MoveTo(onScreenPosition, offScreenPosition, duration);
                animationCooldown = duration + 0.2f;
                activated = !activated;
                GameHandler.currentState = GameState.NORMALGAME;

                // Added 20-05-28
                UIManager.deactivatePuzzleControlsPanel();
                ////
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!puzzleSolved)
            {
                PlayerController.canHackGuard = false;
            }
            if(interactionButtonSubject == null)
            {
                Debug.Log("Interaction subject is null");
            }
            canBeActivated = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.canHackGuard = true;
            interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.SQUARE);
            canBeActivated = false;
        }
    }

    public void DeactivatePuzzle()
    {
        // Added 20-05-28
        UIManager.deactivatePuzzleControlsPanel();
        ////

        audioManager.Play("CompletePuzzle");
        puzzleSolved = true;

        //Debug.Log("Deactivated");
        MoveTo(onScreenPosition, offScreenPosition, duration, deactivationDelay);
        animationCooldown = duration + deactivationDelay + 1.5f;
        activated = !activated;
        GameHandler.currentState = GameState.NORMALGAME;
    }

    void MoveTo(Vector3 origin, Vector3 target, float duration)
    {
        StartCoroutine(AnimateMove(origin, target, duration));
    }

    void MoveTo(Vector3 origin, Vector3 target, float duration, float delay)
    {
        StartCoroutine(AnimateMove(origin, target, duration, delay));
    }

    IEnumerator AnimateMove(Vector3 origin, Vector3 target, float duration)
    {
        float journey = 0f;
        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);

            float curvePercent = animationCurve.Evaluate(percent);
            objectToMove.transform.position = Vector3.LerpUnclamped(origin, target, curvePercent);

            yield return null;
        }
    }
    IEnumerator AnimateMove(Vector3 origin, Vector3 target, float duration,float delay)
    {
        float journey = 0f;
        float delayCounter = 0f;

        while(delayCounter <= delay)
        {
            delayCounter = delayCounter + Time.deltaTime;
            yield return null;
        }

        while (journey <= duration)
        {
            journey = journey + Time.deltaTime;
            float percent = Mathf.Clamp01(journey / duration);

            float curvePercent = animationCurve.Evaluate(percent);
            objectToMove.transform.position = Vector3.LerpUnclamped(origin, target, curvePercent);

            yield return null;
        }
    }

    public void PlayerSpottedNotify(Vector3 position)
    {
        if(activated)
        {
            canBeActivated = false;
            audioManager.Play("ActivateDoorPuzzle");
            Debug.Log("FORCED TO CLOSE DOWN THE PUZZLE SOLVING DUE TO BEING SPOTTED");
            animationCooldown = duration + 0.2f;
            //MoveTo(onScreenPosition, offScreenPosition, duration);
            MoveTo(onScreenPosition, offScreenPosition, 0.2f);
            activated = !activated;
            GameHandler.currentState = GameState.NORMALGAME;
            UIManager.deactivatePuzzleControlsPanel();
        }
    }

    private void OnDestroy()
    {
        playerSpottedSubject.RemoveObserver(this);
    }
}
