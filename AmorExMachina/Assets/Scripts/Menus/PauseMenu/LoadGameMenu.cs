using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGameMenu : MonoBehaviour
{

    [SerializeField] PauseMenu pauseMenu = null;
    [HideInInspector] public GameObject firstSelectedButtonInLoadMenu = null;
    [HideInInspector] public EventSystem eventSystem = null;

    private SceneHandler sceneHandler = null;

    [SerializeField] private Button loadFromFileButton = null;
    [SerializeField] private Button restartLevelButton = null;
    [SerializeField] private Button backButton = null;

    private TextMeshProUGUI loadFromFileText = null;
    private TextMeshProUGUI restartLevelText = null;
    private TextMeshProUGUI backText = null;

    private void Start()
    {
        sceneHandler = SceneHandler.instance;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        transform.gameObject.SetActive(false);

        firstSelectedButtonInLoadMenu = restartLevelButton.gameObject;

        loadFromFileText = loadFromFileButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        restartLevelText = restartLevelButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        backText = backButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            ExitLoadMenu();
        }

        if(SceneHandler.hasSaveToAFile)
        {
            firstSelectedButtonInLoadMenu = loadFromFileButton.gameObject;
        }
        else
        {
            firstSelectedButtonInLoadMenu = restartLevelButton.gameObject;
        }

        SelectedButton();
        loadFromFileButton.gameObject.SetActive(SceneHandler.hasSaveToAFile);
    }

    void SelectedButton()
    {
        loadFromFileText.color = Color.white;
        restartLevelText.color = Color.white;
        backText.color = Color.white;

        if (eventSystem.currentSelectedGameObject == loadFromFileButton.gameObject)
        {
            loadFromFileText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if(eventSystem.currentSelectedGameObject == restartLevelButton.gameObject)
        {
            restartLevelText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if(eventSystem.currentSelectedGameObject == backButton.gameObject)
        {
            backText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }
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
