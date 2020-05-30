using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    SceneHandler sceneHandler = null;
    [SerializeField] OptionsMenu optionsMenu = null;
    [SerializeField] EventSystem eventSystem = null;
    AudioSource mainMenuAudio = null;

    public GameObject lastSelectedButton = null;
    public GameObject currentSelectedButton = null;
    [SerializeField] AudioSource buttonAudio = null;

    private void Awake()
    {
        mainMenuAudio = GetComponent<AudioSource>();
        sceneHandler = SceneHandler.instance;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        currentSelectedButton = eventSystem.firstSelectedGameObject;
        lastSelectedButton = currentSelectedButton;
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        mainMenuAudio.volume = optionsMenu.settings.musicVolume * optionsMenu.settings.masterVolume;
        buttonAudio.volume = optionsMenu.settings.effectsVolume * optionsMenu.settings.masterVolume;

        if (lastSelectedButton != eventSystem.currentSelectedGameObject && eventSystem.currentSelectedGameObject != null)
        {
            if (!buttonAudio.isPlaying)
                buttonAudio.Play();

            lastSelectedButton = eventSystem.currentSelectedGameObject;
        }

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelectedButton);
        }
        else
        {
            lastSelectedButton = EventSystem.current.currentSelectedGameObject;
        }
    }

    public void NewGameButton()
    {
        sceneHandler.StartLoadNextSceneCoroutine();
    }

    public void OptionsButton()
    {
        StartCoroutine("SwitchToOptions");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    IEnumerator SwitchToOptions()
    {
        optionsMenu.gameObject.SetActive(true);
        currentSelectedButton = eventSystem.currentSelectedGameObject;
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(optionsMenu.firstSelectedButtonInOptions);
        transform.GetChild(0).gameObject.SetActive(false);
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
