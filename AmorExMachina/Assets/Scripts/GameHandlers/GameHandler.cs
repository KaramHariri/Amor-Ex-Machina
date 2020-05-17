using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public enum GameState
{
    NORMALGAME,
    MENU,
    PUZZLE,
    HACKING,
    WON,
    LOST
}

public enum PlayerState
{
    SPOTTED,
    NOTSPOTTED,
    CAUGHT
}

public class GameHandler : MonoBehaviour
{
    static public GameState currentState;
    [SerializeField] private PlayerState playerState = PlayerState.NOTSPOTTED;
    static public GameState previousState;

    private EventSystem eventSystem = null;

    public static Guard[] guards;

    private AudioManager audioManager = null;
    [SerializeField] private PlayerVariables playerVariables = null;

    private SceneHandler sceneHandler = null;

    public static Action reloadSceneButton = delegate { };

    private float inputDelay = 0.0f;

    private void Awake()
    {
        FindAllGuards();
        currentState = GameState.NORMALGAME;
        previousState = currentState;
        audioManager = FindObjectOfType<AudioManager>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        sceneHandler = SceneHandler.instance;

        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetButtonDown("Options") && currentState != GameState.MENU && inputDelay >= 0.2f)
        {
            StartCoroutine("SetPauseMenuSelectedButton");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            currentState = GameState.MENU;
        }

        if(currentState == GameState.MENU)
        {
            inputDelay = 0.0f;
        }
        else
        {
            inputDelay += Time.deltaTime;
            inputDelay = Mathf.Clamp(inputDelay, 0.0f, 5.0f);
        }

        if(previousState != currentState && currentState != GameState.MENU)
        {
            //Debug.Log("Switching state from :" + previousState + " to " + currentState);
            previousState = currentState;
        }

        GuardSpottedPlayerCheck();
        PlayerCaughtCheck();
        audioManager.UpdateBackGroundMusic(playerState);
    }

    public void StartReloadingSceneCoroutine()
    {

    }

    IEnumerator ReloadSceneCoroutine()
    {
        if(sceneHandler != null)
        {
            yield return new WaitForSeconds(3.0f);
            sceneHandler.StartReloadSceneCoroutine();
        }
        else
        {
            yield return new WaitForSeconds(3.0f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void ReloadSceneButton()
    {
        if(sceneHandler != null)
        {
            sceneHandler.StartReloadSceneCoroutine();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void FindAllGuards()
    {
        GameObject[] allGuards = GameObject.FindGameObjectsWithTag("Guard");
        guards = new Guard[allGuards.Length];
        for (int i = 0; i < allGuards.Length; i++)
        {
            guards[i] = allGuards[i].GetComponent<Guard>();
        }
    }

    bool GuardSpottedPlayerCheck()
    {
        if (playerState != PlayerState.CAUGHT)
        {
            for (int i = 0; i < guards.Length; i++)
            {
                if (guards[i].sensing.PlayerDetectedCheck())
                {
                    playerState = PlayerState.SPOTTED;
                    return true;
                }
            }
            playerState = PlayerState.NOTSPOTTED;
        }
        return false;
    }

    bool PlayerCaughtCheck()
    {
        if(playerVariables.caught)
        {
            playerState = PlayerState.CAUGHT;
            return true;
        }
        return false;
    }

    IEnumerator SetPauseMenuSelectedButton()
    {
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(eventSystem.firstSelectedGameObject);
    }

    private void OnEnable()
    {
        reloadSceneButton += ReloadSceneButton;
    }

    private void OnDestroy()
    {
        reloadSceneButton -= ReloadSceneButton;
    }

}
