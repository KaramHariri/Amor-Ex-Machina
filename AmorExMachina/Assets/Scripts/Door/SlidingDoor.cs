using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour
{
    AudioManager audioManager;

    public enum slidingDoorState { NONE, OPEN, CLOSE }

    [Header("References")]
    [SerializeField] Transform rightDoor = null;
    [SerializeField] Transform leftDoor = null;

    [Header("Settings")]
    [SerializeField] LayerMask LayersToDetect = 0;
    [Range(1, 10)]
    [SerializeField] float speed = 5;
    [Range(0.1f, 4.0f)]
    [SerializeField] float delay = 1;

    [SerializeField] float closetXPos = 0.0f;
    [SerializeField] float openXPos = 0.0f;

    #region private
    private bool animating = false;
    private slidingDoorState animatingState = slidingDoorState.NONE;
    private slidingDoorState state = slidingDoorState.NONE;

    private List<Transform> inRange = new List<Transform>();

    IEnumerator IE_StartAnimating = null, IE_Animate = null, IE_LeftDoor = null, IE_RightDoor = null;
    #endregion

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        //closetXPos = Mathf.Abs(leftDoor.transform.position.x);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & LayersToDetect) == 0) { return; }

        if (other is SphereCollider) { return; }
        inRange.Add(other.transform);
        state = slidingDoorState.OPEN;
        audioManager.Play("DoorOpen", this.transform.position);
        StartAnimating();
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & LayersToDetect) == 0) { return; }
        if(other is SphereCollider) { return; }

        inRange.Remove(other.transform);
        if (inRange.Count <= 0)
        {
            audioManager.Play("DoorOpen", this.transform.position);
            state = slidingDoorState.CLOSE;
            StartAnimating();
        }
    }

    private void StartAnimating()
    {
        //Debug.Log("StartAnimating()");
        if (IE_StartAnimating != null)
        {
            StopCoroutine(IE_StartAnimating);
        }

        IE_StartAnimating = Begin();
        StartCoroutine(IE_StartAnimating);
    }

    private IEnumerator Begin()
    {
        while (animating) { yield return null; }

        if (IE_Animate != null) { StopCoroutine(IE_Animate); }
        IE_Animate = Animate(state.Equals(slidingDoorState.OPEN) ? openXPos : closetXPos);
        //Debug.Log("Begin() - StartCoroutine " + (state.Equals(slidingDoorState.OPEN) ? "Open" : "close"));
        StartCoroutine(IE_Animate);
    }

    private IEnumerator Animate(float xPos)
    {
        //Debug.Log("Animate()" + xPos);

        if (Approximately(leftDoor.transform.position.x, xPos, 0.001f)) { yield break; }
        //Debug.Log("Animate() changed state to " + state);
        animatingState = state;

        yield return new WaitForSeconds(animatingState.Equals(slidingDoorState.CLOSE) ? delay / 2 : delay);

        //Debug.Log("Waited for the delay");

        if (IE_RightDoor != null) { StopCoroutine(IE_RightDoor); }
        if (IE_LeftDoor != null) { StopCoroutine(IE_LeftDoor); }

        IE_RightDoor = Move(rightDoor, -xPos);
        IE_LeftDoor = Move(leftDoor, xPos);

        //audioManager.Play(state.Equals(slidingDoorState.OPEN) ? "OpenDoor" : "CloseDoor");

        StartCoroutine(IE_RightDoor);
        StartCoroutine(IE_LeftDoor);

        while (animating)
        {
            //Debug.Log("Should be animating");
            yield return null;
        }
    }

    private IEnumerator Move(Transform transform, float xPos)
    {
        //Debug.Log("Move()");

        animating = true;
        while (!Approximately(transform.localPosition.x, xPos, 0.001f))
        {
            float newXPos = transform.localPosition.x;
            newXPos = Mathf.Lerp(newXPos, xPos, speed * Time.deltaTime);
            transform.localPosition = new Vector3(newXPos, transform.localPosition.y, transform.localPosition.z);

            //Debug.Log("Move()" + newXPos);

            yield return null;
        }
        animating = false;
    }

    private bool Approximately(float a, float b, float t)
    {
        return Mathf.Abs(a - b) <= t;
    }

    public void UnlockDoor()
    {
        LayersToDetect |= (1 << LayerMask.NameToLayer("Player"));
    }
}
