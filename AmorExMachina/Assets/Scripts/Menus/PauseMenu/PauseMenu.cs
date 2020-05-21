using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] PauseMenuSettings PauseMenuSettings = null;
    [SerializeField] LoadGameMenu loadGameMenu = null;
    private GameObject pauseMenuObject = null;
    private EventSystem eventSystem = null;

    [HideInInspector] public bool switchedToSettings = false;

    GameObject currentSelectedButton = null;

    private bool canTakeInput = false;

    private void Start()
    {
        Cursor.visible = false;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        pauseMenuObject = transform.GetChild(0).gameObject;
        pauseMenuObject.SetActive(false);
        canTakeInput = false;
    }

    private void Update()
    {
        if(GameHandler.currentState == GameState.MENU)
        {
            if (!switchedToSettings && !pauseMenuObject.activeInHierarchy)
            {
                pauseMenuObject.SetActive(true);
                canTakeInput = true;
                return;
            }
        }

        if(Input.GetButtonDown("Cancel") && canTakeInput)
        {
            //Resume();
            StartCoroutine("ResumeGame");
        }
    }

    public void Resume()
    {
        pauseMenuObject.SetActive(false);
        GameHandler.currentState = GameHandler.previousState;
    }

    public void LoadGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //GameHandler.reloadSceneButton();
        StartCoroutine("SwitchToLoad");
    }

    public void Settings()
    {
        StartCoroutine("SwitchToSettings");
    }

    public void Quit()
    {
        Application.Quit();
    }

    IEnumerator SwitchToSettings()
    {
        PauseMenuSettings.gameObject.SetActive(true);
        currentSelectedButton = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(PauseMenuSettings.firstSelectedButtonInOptions);
        switchedToSettings = true;
        pauseMenuObject.gameObject.SetActive(false);
        canTakeInput = false;
    }

    IEnumerator SwitchToLoad()
    {
        loadGameMenu.gameObject.SetActive(true);
        currentSelectedButton = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(loadGameMenu.firstSelectedButtonInLoadMenu);
        switchedToSettings = true;
        pauseMenuObject.gameObject.SetActive(false);
        canTakeInput = false;
    }

    public void StartSetSelectedButtonEnumerator()
    {
        StartCoroutine("SetSelectedButton");
    }

    IEnumerator SetSelectedButton()
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(currentSelectedButton);
        canTakeInput = true;
    }

    IEnumerator ResumeGame()
    {
        canTakeInput = false;
        Resume();
        yield return null;
        canTakeInput = true;
    }
}
