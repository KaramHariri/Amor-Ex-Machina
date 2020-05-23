using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveHandler : MonoBehaviour
{
    void Start()
    {
        GameEvents.current.onSaveDataEvent += SavePlayerData;
        GameEvents.current.onLoadDataEvent += LoadPlayerData;
    }

    void SavePlayerData()
    {
        //Debug.Log("Saving player data");
        SaveData.current.playerData.position = transform.position;
        SaveData.current.playerData.rotation = transform.rotation;
        PlayerController temp = GetComponent<PlayerController>();
        if(temp.disabledGuard != null)
        {
            Debug.Log("Sibling index = " + temp.disabledGuard.transform.GetSiblingIndex());
            SaveData.current.playerData.disabledIndex = temp.disabledGuard.transform.GetSiblingIndex();
        }
    }

    void LoadPlayerData()
    {
        //Debug.Log("Loading player data");
        transform.position = SaveData.current.playerData.position;
        transform.rotation = SaveData.current.playerData.rotation;
        if(SaveData.current.playerData.disabledIndex != -1)
        {
            PlayerController temp = GetComponent<PlayerController>();
            Debug.Log("sibling index = " + SaveData.current.playerData.disabledIndex);
            temp.disabledGuard = GameObject.Find("GuardsHolder").transform.GetChild(SaveData.current.playerData.disabledIndex).GetComponent<Guard>();
        }
    }
}
