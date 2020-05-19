using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    SceneHandler sceneHandler = null;
    [SerializeField] OptionsMenu optionsMenu = null;
    [SerializeField] EventSystem eventSystem = null;
    AudioSource mainMenuAudio = null;

    GameObject currentSelectedButton = null;

    private void Awake()
    {
        mainMenuAudio = GetComponent<AudioSource>();
        sceneHandler = SceneHandler.instance;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        mainMenuAudio.volume = optionsMenu.settings.musicVolume * optionsMenu.settings.masterVolume;
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
