using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private Image backgroundImage = null;

    private GameObject resumeGameObject = null;
    private GameObject loadGameObject = null;
    private GameObject settingsGameObject = null;
    private GameObject quitGameObject = null;

    private TextMeshProUGUI resumeText = null;
    private TextMeshProUGUI loadText = null;
    private TextMeshProUGUI settingsText = null;
    private TextMeshProUGUI quitText = null;

    private void Start()
    {
        Cursor.visible = false;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        pauseMenuObject = transform.GetChild(0).gameObject;
        pauseMenuObject.SetActive(false);
        canTakeInput = false;
        currentSelectedButton = eventSystem.firstSelectedGameObject;
        lastSelectedButton = currentSelectedButton;

        backgroundImage = GetComponent<Image>();
        backgroundImage.enabled = false;

        audioManager = GameHandler.audioManager;
        if(audioManager == null)
        {
            Debug.Log("PauseMenu can't find AudioManager in GameHandler");
        }
        InitButtonsGameObject();
        InitButtonsText();
    }

    private void Update()
    {
        if(GameHandler.currentState == GameState.MENU)
        {
            if (!switchedToSettings && !pauseMenuObject.activeInHierarchy)
            {
                pauseMenuObject.SetActive(true);
                backgroundImage.enabled = true; ;
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

        SelectedButton();

        if (Input.GetButtonDown("Cancel") && canTakeInput)
        {
            //Resume();
            StartCoroutine("ResumeGame");
        }
    }

    void SelectedButton()
    {
        resumeText.color = Color.white;
        loadText.color = Color.white;
        settingsText.color = Color.white;
        quitText.color = Color.white;

        if (eventSystem.currentSelectedGameObject == resumeGameObject)
        {
            resumeText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == loadGameObject)
        {
            loadText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == settingsGameObject)
        {
            settingsText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
            return;
        }

        if (eventSystem.currentSelectedGameObject == quitGameObject)
        {
            quitText.color = new Color(1.0f, 0.5176471f, 0.08627451f, 1.0f);
        }
    }

    public void Resume()
    {
        //pauseMenuObject.SetActive(false);
        //backgroundImage.enabled = false;
        //GameHandler.currentState = GameHandler.previousState;
        StartCoroutine("ResumeGame");
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
        yield return null;
        canTakeInput = false;
        pauseMenuObject.SetActive(false);
        backgroundImage.enabled = false;
        GameHandler.currentState = GameHandler.previousState;
        yield return null;
    }

    void InitButtonsGameObject()
    {
        resumeGameObject = pauseMenuObject.transform.GetChild(0).gameObject;
        loadGameObject = pauseMenuObject.transform.GetChild(1).gameObject;
        settingsGameObject = pauseMenuObject.transform.GetChild(2).gameObject;
        quitGameObject = pauseMenuObject.transform.GetChild(3).gameObject;
    }

    void InitButtonsText()
    {
        resumeText = resumeGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        loadText = loadGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        settingsText = settingsGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        quitText = quitGameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }
}
