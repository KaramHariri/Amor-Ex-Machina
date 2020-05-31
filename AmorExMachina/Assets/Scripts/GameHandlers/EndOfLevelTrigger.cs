using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfLevelTrigger : MonoBehaviour
{
    [SerializeField] private bool thirdLevel = false;
    [SerializeField] private float delayTimer = 5.0f;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine("DelayEndOfThirdLevel");
        }
    }

    IEnumerator DelayEndOfThirdLevel()
    {
        if (thirdLevel)
        {
            yield return new WaitForSeconds(delayTimer);
        }
        else
            yield return new WaitForSeconds(1.0f);

        SceneHandler.loadNextScene();
    }
}
