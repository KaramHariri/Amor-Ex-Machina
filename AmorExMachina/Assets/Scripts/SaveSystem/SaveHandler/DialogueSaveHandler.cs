using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogueSaveHandler : MonoBehaviour
{

    //private bool hasUpdated = false;
    [SerializeField] private bool hasBeenInitialized = false;
    private float initializeTimer = 0f;
    private DialogueData data = new DialogueData();
    Dialogue dialogueScript;

    // Start is called before the first frame update
    void Start()
    {
        dialogueScript = GetComponent<Dialogue>();
        data.id = SceneManager.GetActiveScene().name + transform.parent.GetSiblingIndex();
        //Debug.Log("checkpoint id:" + data.id);

        if (!SaveData.current.dialogues.ContainsKey(data.id))
        {
            //Debug.Log("Added checkpoint to checkpoints");
            SaveData.current.dialogues.Add(data.id, data);

            //GameEvents.current.onSaveDataEvent += SaveCheckpointData;
        }
        else
        {
            //Debug.Log("Already have a checkpoint with this ID");
            //LoadCheckpointData();
        }

        GameEvents.current.onSaveDataEvent += SaveDialogueData;
        GameEvents.current.onLoadDataEvent += LoadDialogueData;
    }

    public void Update()
    {
        if(!hasBeenInitialized)
        {
            initializeTimer += Time.deltaTime;
            if (initializeTimer > 0.2f)
            {
                hasBeenInitialized = true;
                dialogueScript.hasBeenInitialized = true;
            }
        }
    }

    private void SaveDialogueData()
    {
        Debug.Log("Saving dialogue data: name" +transform.name + " has played dialogue " + dialogueScript.dialoguePlayed);
        SaveData.current.dialogues[data.id].hasPlayed = dialogueScript.dialoguePlayed;
    }

    private void LoadDialogueData()
    {
        Debug.Log("loading dialogue data: name" + transform.name + " has played dialogue " + dialogueScript.dialoguePlayed);
        hasBeenInitialized = true;
        dialogueScript.hasBeenInitialized = true;
        dialogueScript.dialoguePlayed = SaveData.current.dialogues[data.id].hasPlayed;
    }
}
