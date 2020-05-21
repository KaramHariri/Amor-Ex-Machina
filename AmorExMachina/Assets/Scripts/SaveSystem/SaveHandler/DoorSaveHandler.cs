using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSaveHandler : MonoBehaviour
{
    public SlidingDoor slidingDoorScript;
    public PuzzleActivator PA;
    DoorData data = new DoorData();

    void Start()
    {
        //slidingDoorScript = GetComponent<SlidingDoor>();
        data.id = SceneManager.GetActiveScene().name + transform.GetSiblingIndex();
        if (!SaveData.current.doors.ContainsKey(data.id))
        {
            SaveData.current.doors.Add(data.id, data);
            //Debug.Log("Added door save handler");
        }
        else
        {
            //LoadDoorData();
            //Debug.Log("Already have an object with this ID"); 
        }

        GameEvents.current.onSaveDataEvent += SaveDoorData;
        GameEvents.current.onLoadDataEvent += LoadDoorData;
    }


    private void SaveDoorData()
    {
        //Debug.Log("Saving door data");
        SaveData.current.doors[data.id].isUnlocked = slidingDoorScript.isUnlocked;
    }

    private void LoadDoorData()
    {
        //Debug.Log("Loading door data");
        if(SaveData.current.doors[data.id].isUnlocked)
        {
            //Debug.Log("Door" + transform.name + "unlocked");
            slidingDoorScript.UnlockDoor();
            PA.puzzleSolved = true;
        }
    }

    private void OnDestroy()
    {
        //Probably should remove the events

        //GameEvents.current.onSaveDataEvent -= SaveDoorData;
        //GameEvents.current.onLoadDataEvent -= LoadDoorData;
    }
}
