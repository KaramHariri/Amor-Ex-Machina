using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public TMP_InputField saveName;
    public GameObject loadButtonPrefab;

    public string[] saveFiles;
    public void OnSave()
    {
        SerializationManager.Save(saveName.text, SaveData.current);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            GetLoadFiles();
        }
    }

    public void GetLoadFiles()
    {
        if(!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        saveFiles = Directory.GetFiles(Application.persistentDataPath + "/saves/");

        for(int i = 0; i < saveFiles.Length; i++)
        {
            Debug.Log(saveFiles[i]);
        }
    }

}
