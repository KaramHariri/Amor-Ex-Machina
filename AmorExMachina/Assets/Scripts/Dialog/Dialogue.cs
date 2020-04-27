using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue : MonoBehaviour
{
    [TextArea(1, 5)]
    public string sentence;
    public AudioClip sentenceAudio;

    [SerializeField]
    private GameObject dialogueGameObject;
    private Text dialogueText;
    private AudioSource dialogueAudio;
    private bool dialoguePlayed = false;
    private bool finishedTyping = false;


    private void Start()
    {
        dialogueText = dialogueGameObject.GetComponent<Text>();
        dialogueAudio = dialogueGameObject.GetComponent<AudioSource>();
        dialogueGameObject.SetActive(false);
    }

    private void Update()
    {
        DeactivateDialogueCheck();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !dialoguePlayed)
        {
            dialoguePlayed = true;
            dialogueGameObject.SetActive(true);
            dialogueAudio.clip = sentenceAudio;
            dialogueAudio.Play();
            StopAllCoroutines();
            StartCoroutine(TypeSentence());
        }
    }

    IEnumerator TypeSentence()
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
        Debug.Log("finished typing");
        finishedTyping = true;
    }

    void DeactivateDialogueCheck()
    {
        if(dialoguePlayed && !dialogueAudio.isPlaying && finishedTyping)
        {
            dialogueGameObject.SetActive(false);
        }
    }
}
