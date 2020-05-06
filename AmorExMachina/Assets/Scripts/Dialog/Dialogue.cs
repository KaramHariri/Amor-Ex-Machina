using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue : MonoBehaviour
{
    [TextArea(1, 5)]
    public string sentence = "";
    public AudioClip sentenceAudio = null;

    [SerializeField]
    private GameObject dialogueGameObject = null;
    private Text dialogueText = null;
    private AudioSource dialogueAudio = null;
    private bool dialoguePlayed = false;
    private bool finishedTyping = false;

    [SerializeField] private Settings settings = null;

    private void Start()
    {
        dialogueText = dialogueGameObject.GetComponent<Text>();
        dialogueAudio = dialogueGameObject.GetComponent<AudioSource>();
        dialogueGameObject.SetActive(false);
        dialogueAudio.volume = settings.voiceVolume * settings.masterVolume;
        dialogueAudio.loop = false;
        dialogueAudio.playOnAwake = false;
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
        if (settings.subtitle)
        {
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return null;
            }
            Debug.Log("finished typing");
            finishedTyping = true;
        }
    }

    void DeactivateDialogueCheck()
    {
        if(dialoguePlayed && !dialogueAudio.isPlaying && finishedTyping)
        {
            dialogueGameObject.SetActive(false);
        }
    }
}
