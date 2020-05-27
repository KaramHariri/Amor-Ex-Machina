using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    private TextMeshProUGUI dialogueText = null;
    private AudioSource dialogueAudio = null;
    private bool dialoguePlayed = false;
    private bool finishedTyping = false;

    [SerializeField] private float textAnimationSpeed = 0.1f;

    //[SerializeField] private Settings settings = null;
    private Settings settings = null;

    private void Start()
    {
        settings = GameHandler.settings;
        if(settings == null)
        {
            Debug.Log("Dialogue can't find Settings in GameHandler");
        }

        dialogueText = dialogueGameObject.GetComponent<TextMeshProUGUI>();
        dialogueAudio = dialogueGameObject.GetComponent<AudioSource>();
        dialogueGameObject.SetActive(false);
        dialogueAudio.volume = settings.voiceVolume * settings.masterVolume;
        dialogueAudio.loop = false;
        dialogueAudio.playOnAwake = false;
    }

    private void Update()
    {
        if(GameHandler.currentState == GameState.MENU )
        {
            dialogueAudio.Pause();
            return;
        }
        else
        {
            if(dialoguePlayed)
            {
                dialogueAudio.UnPause();
            }
        }
        DeactivateDialogueCheck();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !dialoguePlayed)
        {
            dialoguePlayed = true;
            dialogueGameObject.SetActive(true);
            dialogueAudio.clip = sentenceAudio;
            dialogueAudio.volume = settings.voiceVolume * settings.masterVolume;
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
                yield return new WaitForSeconds(textAnimationSpeed);
            }
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
