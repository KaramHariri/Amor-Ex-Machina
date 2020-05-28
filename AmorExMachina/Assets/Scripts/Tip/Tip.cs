using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tip : MonoBehaviour
{
    private GameObject dialogueGameObject = null;
    public float showTime = 3.0f;
    private float showTimeTimer;
    private bool activateTimer;
    private bool hasActivated = false;

    void Start()
    {
        dialogueGameObject = transform.parent.Find("TipCanvas").gameObject;
        dialogueGameObject.SetActive(false);
    }

    void Update()
    {
        if(activateTimer)
        {
            showTimeTimer -= Time.deltaTime;
            if(showTimeTimer > 0)
            {
                dialogueGameObject.SetActive(true);
            }
            else
            {
                dialogueGameObject.SetActive(false);
                hasActivated = true;
                activateTimer = false;
                showTimeTimer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !hasActivated)
        {
            activateTimer = true;
            showTimeTimer = showTime; 
        }
    }


}
