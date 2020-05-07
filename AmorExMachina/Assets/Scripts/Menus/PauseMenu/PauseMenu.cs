using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] PauseMenuSettings PauseMenuSettings = null;
    private GameObject pauseMenuObject = null;
    private EventSystem eventSystem = null;

    [HideInInspector] public bool switchedToSettings = false;

    GameObject currentSelectedButton = null;

    private void Start()
    {
        Cursor.visible = false;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        pauseMenuObject = transform.GetChild(0).gameObject;
        pauseMenuObject.SetActive(false);
    }

    private void Update()
    {
        if(GameHandler.currentState == GameState.MENU)
        {
            if(!switchedToSettings)
                pauseMenuObject.SetActive(true);
        }
    }

    public void Resume()
    {
        pauseMenuObject.SetActive(false);
        GameHandler.currentState = GameHandler.previousState;
    }

    public void LoadGame()
    {
        Debug.Log("Load Game Pressed !");
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
    }
}
