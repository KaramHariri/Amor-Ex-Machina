using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] PauseMenuSettings PauseMenuSettings = null;
    [SerializeField] LoadGameMenu loadGameMenu = null;
    private GameObject pauseMenuObject = null;
    private EventSystem eventSystem = null;

    [HideInInspector] public bool switchedToSettings = false;

    GameObject currentSelectedButton = null;
    GameObject lastSelectedButton = null;

    private bool canTakeInput = false;

    private AudioManager audioManager = null;

    private void Start()
    {
        Cursor.visible = false;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        pauseMenuObject = transform.GetChild(0).gameObject;
        pauseMenuObject.SetActive(false);
        canTakeInput = false;
        currentSelectedButton = eventSystem.firstSelectedGameObject;
        lastSelectedButton = currentSelectedButton;

        audioManager = GameHandler.audioManager;
        if(audioManager == null)
        {
            Debug.Log("PauseMenu can't find AudioManager in GameHandler");
        }
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

        if(eventSystem.currentSelectedGameObject != lastSelectedButton)
        {
            audioManager.Play("SwitchMenuButton");
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }


        if (Input.GetButtonDown("Cancel") && canTakeInput)
        {
            //Resume();
            StartCoroutine("ResumeGame");
        }
    }

    public void Resume()
    {
        pauseMenuObject.SetActive(false);
        //audioManager.Play("MenuButtonPressed");
        GameHandler.currentState = GameHandler.previousState;
    }

    public void LoadGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //GameHandler.reloadSceneButton();
        //audioManager.Play("MenuButtonPressed");
        StartCoroutine("SwitchToLoad");
    }

    public void Settings()
    {
        //audioManager.Play("MenuButtonPressed");
        StartCoroutine("SwitchToSettings");
    }

    public void Quit()
    {
        //audioManager.Play("MenuButtonPressed");
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
