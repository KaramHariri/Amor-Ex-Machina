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
        Debug.Log("Saving player data");
        SaveData.current.playerData.position = transform.position;
        SaveData.current.playerData.rotation = transform.rotation;
    }

    void LoadPlayerData()
    {
        Debug.Log("Loading player data");
        transform.position = SaveData.current.playerData.position;
        transform.rotation = SaveData.current.playerData.rotation;
    }
}
