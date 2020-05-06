using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Guard))]
public class GuardSaveHandler : MonoBehaviour
{
    Guard guardScript;
    GuardData data = new GuardData();
    int indexInGuardData = 0;

    void Start()
    {
        Debug.Log("Added guard to guardData");

        guardScript = GetComponent<Guard>();

        indexInGuardData = SaveData.current.guardData.Count;
        SaveData.current.guardData.Add(data);

        GameEvents.current.onSaveDataEvent += SaveGuardData;
        GameEvents.current.onLoadDataEvent += LoadGuardData;
    }


    private void SaveGuardData()
    {
        Debug.Log("Guard updating data");
        //Debug.Log("Position: (" + transform.position.x + ", " + transform.position.y + ", " + transform.position.z + ")");
        //Debug.Log("rotation: " + transform.rotation.eulerAngles);

        //data.position = transform.position;
        //data.rotation = transform.rotation;
        //data.isDisabled = guardScript.disabled;

        SaveData.current.guardData[indexInGuardData].position = transform.position;
        SaveData.current.guardData[indexInGuardData].rotation = transform.rotation;
        SaveData.current.guardData[indexInGuardData].isDisabled = guardScript.disabled;
    }

    private void LoadGuardData()
    {
        Debug.Log("Guard loading data");
        transform.position = SaveData.current.guardData[indexInGuardData].position;
        transform.rotation = SaveData.current.guardData[indexInGuardData].rotation;
        guardScript.disabled = SaveData.current.guardData[indexInGuardData].isDisabled;
        guardScript.sensing.Reset();
    }
}
