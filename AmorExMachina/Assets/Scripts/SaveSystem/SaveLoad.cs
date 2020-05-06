using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    private bool hasUpdated = false;
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    Debug.Log("Sent event out so everything saves its data");
        //    Save();
        //}

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Loaded from file");
            Load();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!hasUpdated && other.CompareTag("Player"))
        {
            Debug.Log("Collided with player, saving game");
            Save();
            hasUpdated = true;
        }
    }

    public void Save()
    {
        StartCoroutine(SaveGame());
    }

    private void SaveToFile()
    {
        SerializationManager.Save("test", SaveData.current);
    }

    IEnumerator SaveGame()
    {
        //Notify all listerners that they should update and save their data
        GameEvents.current.SaveDataEvent();

        //yield return new WaitForSeconds(0.2f);
        yield return null;

        SaveToFile();
    }

    public void Load()
    {
        SaveData.current = (SaveData)SerializationManager.Load(Application.persistentDataPath + "/saves/" + "test" + ".save");
        GameEvents.current.LoadDataEvent();
    }

}
