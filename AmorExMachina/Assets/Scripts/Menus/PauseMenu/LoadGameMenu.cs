using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameMenu : MonoBehaviour
{

    [SerializeField] PauseMenu pauseMenu = null;
    public GameObject firstSelectedButtonInLoadMenu = null;
    [HideInInspector] public EventSystem eventSystem = null;

    private SceneHandler sceneHandler = null;

    [SerializeField] private Button loadFromFileButton = null;

    private void Start()
    {
        sceneHandler = SceneHandler.instance;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        transform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ExitLoadMenu();
        }

        loadFromFileButton.interactable = SceneHandler.hasSaveToAFile;
    }

    public void LoadSaveFile()
    {
        if(sceneHandler != null)
            sceneHandler.LoadFromFile();
    }

    public void ReloadScene()
    {
        if (sceneHandler != null)
            sceneHandler.StartReloadSceneCoroutine();
    }

    public void ExitLoadMenu()
    {
        StartCoroutine("SwitchToPauseMenu");
    }

    IEnumerator SwitchToPauseMenu()
    {
        pauseMenu.transform.GetChild(0).gameObject.SetActive(true);
        yield return null;
        transform.gameObject.SetActive(false);
        pauseMenu.switchedToSettings = false;
        pauseMenu.StartSetSelectedButtonEnumerator();
    }
}
