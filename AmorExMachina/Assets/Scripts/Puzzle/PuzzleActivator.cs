using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleActivator : MonoBehaviour
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
    private float deactivationDelay = 1.5f;

    private bool canBeActivated = false;

    public void Update()
    {
        if(puzzleSolved) { return; }

        animationCooldown = animationCooldown - Time.deltaTime;
        if (animationCooldown < 0)
            animationCooldown = 0;

        if (!canBeActivated) { return; }

        if (Input.GetButtonDown("Circle") && animationCooldown <= 0)
        {
            if (!activated)
            {
                //Debug.Log("Activated");
                MoveTo(offScreenPosition, onScreenPosition, duration);
                animationCooldown = duration + 0.2f;
                activated = !activated;
            }
            else
            {
                //Debug.Log("Deactivated");
                MoveTo(onScreenPosition, offScreenPosition, duration);
                animationCooldown = duration + 0.2f;
                activated = !activated;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canBeActivated = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canBeActivated = false;
        }
    }

    public void DeactivatePuzzle()
    {
        puzzleSolved = true;

        //Debug.Log("Deactivated");
        MoveTo(onScreenPosition, offScreenPosition, duration, deactivationDelay);
        animationCooldown = duration + deactivationDelay + 1.5f;
        activated = !activated;
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
}
