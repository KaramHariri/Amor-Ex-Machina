﻿using System.Collections;
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
    private bool puzzleSolved = false;

    public float duration = 1.5f;
    public float animationCooldown = 0f;
    private float deactivationDelay = 0.2f;

    private bool canBeActivated = false;

    AudioManager audioManager;
    [SerializeField] PlayerSpottedSubject PlayerSpottedSubject = null;
    [SerializeField] InteractionButtonSubject interactionButtonSubject = null;
    [SerializeField] PlayerVariables playerVariables = null;
    [SerializeField] Settings settings = null;

    private Transform player = null;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        player = playerVariables.playerTransform.Find("character_gabriel");
        if (PlayerSpottedSubject != null)
        {
            PlayerSpottedSubject.AddObserver(this);
        }
        else
        {
            Debug.Log("Could not find PlayerLastSightPositionSubject, add it in the inspector");
        }
    }

    public void Update()
    {
        if(puzzleSolved) { return; }

        animationCooldown = animationCooldown - Time.deltaTime;
        if (animationCooldown < 0)
            animationCooldown = 0;

        if (!canBeActivated) { return; }

        //if (Input.GetButtonDown("Circle") && animationCooldown <= 0)
        if ((Input.GetKeyDown(settings.activatePuzzleController) || Input.GetKeyDown(settings.activatePuzzleKeyboard)) && animationCooldown <= 0)
        {
            Vector3 directionToLockFromPlayer = transform.position - playerVariables.playerTransform.position;
            directionToLockFromPlayer.y = 0;
            directionToLockFromPlayer.Normalize();
            Vector3 playerForwardDirection = player.transform.forward;
            playerForwardDirection.y = 0;
            //Debug.Log("DirToLock" + directionToLockFromPlayer);
            Debug.DrawRay(playerVariables.playerTransform.position, directionToLockFromPlayer, Color.red);
            Debug.DrawRay(playerVariables.playerTransform.position, playerForwardDirection, Color.blue);
            Debug.Log(Vector3.Angle(directionToLockFromPlayer, playerForwardDirection));

            if (Vector3.Angle(directionToLockFromPlayer, playerForwardDirection) > 75) { return; }

            //if(angleToPlayer > 180.0f || angleToPlayer < 90.0f) { return; }
            if (!activated)
            {
                audioManager.Play("ActivateDoorPuzzle");
                //Debug.Log("Activated");
                MoveTo(offScreenPosition, onScreenPosition, duration);
                animationCooldown = duration + 0.2f;
                activated = !activated;
                GameHandler.currentState = GameState.PUZZLE;
            }
            else
            {
                audioManager.Play("ActivateDoorPuzzle");
                //Debug.Log("Deactivated");
                MoveTo(onScreenPosition, offScreenPosition, duration);
                animationCooldown = duration + 0.2f;
                activated = !activated;
                GameHandler.currentState = GameState.NORMALGAME;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerVariables.canHackGuard = false;
            if(interactionButtonSubject == null)
            {
                Debug.Log("Interaction subject is null");
            }
            interactionButtonSubject.NotifyToShowInteractionButton(InteractionButtons.CIRCLE);
            canBeActivated = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerVariables.canHackGuard = true;
            interactionButtonSubject.NotifyToHideInteractionButton(InteractionButtons.CIRCLE);
            canBeActivated = false;
        }
    }

    public void DeactivatePuzzle()
    {
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
            MoveTo(onScreenPosition, offScreenPosition, duration);
            activated = !activated;
            GameHandler.currentState = GameState.NORMALGAME;
        }
    }
}
