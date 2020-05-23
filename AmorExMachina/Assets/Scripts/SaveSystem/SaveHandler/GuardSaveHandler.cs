using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Guard))]
public class GuardSaveHandler : MonoBehaviour
{
    Guard guardScript;
    GuardData data = new GuardData();

    void Start()
    {
        guardScript = GetComponent<Guard>();
        data.id = SceneManager.GetActiveScene().name + guardScript.transform.GetSiblingIndex();
        if(!SaveData.current.guards.ContainsKey(data.id))
        {
            //SaveData.current.guardData.Add(data);
            //Debug.Log("Added guard to guards");
            SaveData.current.guards.Add(data.id, data);
        }
        else 
        {
            //LoadGuardData();
            //Debug.Log("Already have an object with this ID"); 
        }

        GameEvents.current.onSaveDataEvent += SaveGuardData;
        GameEvents.current.onLoadDataEvent += LoadGuardData;
    }


    private void SaveGuardData()
    {
        //Debug.Log("Guard updating data");
        //Debug.Log("Position: (" + transform.position.x + ", " + transform.position.y + ", " + transform.position.z + ")");
        //Debug.Log("rotation: " + transform.rotation.eulerAngles);

        //data.position = transform.position;
        //data.rotation = transform.rotation;
        //data.isDisabled = guardScript.disabled;

        //SaveData.current.guardData[indexInGuardData].position = transform.position;
        //SaveData.current.guardData[indexInGuardData].rotation = transform.rotation;
        //SaveData.current.guardData[indexInGuardData].isDisabled = guardScript.disabled;

        SaveData.current.guards[data.id].position = transform.position;
        SaveData.current.guards[data.id].rotation = transform.rotation;
        SaveData.current.guards[data.id].isDisabled = guardScript.disabled;
        SaveData.current.guards[data.id].siblingIndex = guardScript.transform.GetSiblingIndex();
    }

    private void LoadGuardData()
    {
        //Debug.Log("Guard loading data");
        transform.position = SaveData.current.guards[data.id].position;
        transform.rotation = SaveData.current.guards[data.id].rotation;
        guardScript.disabled = SaveData.current.guards[data.id].isDisabled;
        guardScript.sensing.Reset();
    }
}
