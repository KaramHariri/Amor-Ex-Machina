using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointSaveHandler : MonoBehaviour
{
    //private bool hasUpdated = false;
    [SerializeField] private bool hasBeenInitialized = false;
    [SerializeField] private bool hasUpdated = false;
    private CheckpointData data = new CheckpointData();

    private void Start()
    {
        data.id = SceneManager.GetActiveScene().name + transform.GetSiblingIndex();
        //Debug.Log("checkpoint id:" + data.id);

        if (!SaveData.current.checkpoints.ContainsKey(data.id))
        {
            //Debug.Log("Added checkpoint to checkpoints");
            SaveData.current.checkpoints.Add(data.id, data);

            //GameEvents.current.onSaveDataEvent += SaveCheckpointData;
        }
        else
        {
            //Debug.Log("Already have a checkpoint with this ID");
            //LoadCheckpointData();
        }
        hasBeenInitialized = true;
        GameEvents.current.onLoadDataEvent += LoadCheckpointData;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(hasBeenInitialized && !data.hasUpdated && other.CompareTag("Player"))
        {
            //Debug.Log("Collided with player, saving game");
            data.hasUpdated = true;
            SaveData.current.checkpoints[data.id].hasUpdated = true;
            Save();
            SceneHandler.shouldLoadFromFile = true;
            SceneHandler.hasSaveToAFile = true;
        }
    }

    public void Save()
    {
        StartCoroutine(SaveGame());
    }

    private void SaveToFile()
    {
        //Debug.Log("Saving to file number: " + SceneHandler.instance.GetCurrentSaveFileIndex());
        SerializationManager.Save("test" /*+ SceneHandler.instance.GetCurrentSaveFileIndex()*/ , SaveData.current);
        //SceneHandler.instance.IncreaseSaveFileIndex();
    }

    IEnumerator SaveGame()
    {
        //Notify all listerners that they should update and save their data
        GameEvents.current.SaveDataEvent();

        yield return null;

        SaveToFile();
    }

    private void LoadCheckpointData()
    {
        Debug.Log("Loading checkpoint data");
        hasBeenInitialized = true;
        data.hasUpdated = SaveData.current.checkpoints[data.id].hasUpdated;
        hasUpdated = data.hasUpdated;
    }

    //private void Load()
    //{
    //    SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/" + "test" + ".save");
    //    GameEvents.current.LoadDataEvent();
    //}
}
