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

    public static bool playerIsCaught = false;

    private SceneHandler sceneHandler = null;

    public static Action reloadSceneButton = delegate { };

    private float inputDelay = 0.0f;

    public static AudioManager audioManager = null;
    public static GuardCameraVariables guardCameraVariables = null;
    public static PlayerCamerasVariables playerCamerasVariables = null;
    public static GuardDisabledSubject guardDisabledSubject = null;
    public static PlayerSoundSubject playerSoundSubject = null;
    public static PlayerSpottedSubject playerSpottedSubject = null;
    public static GuardHackedSubject guardHackedSubject = null;
    public static InteractionButtonSubject interactionButtonSubject = null;
    public static Settings settings = null;

    private void Awake()
    {
        FindAllGuards();
        currentState = GameState.NORMALGAME;
        previousState = currentState;
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        sceneHandler = SceneHandler.instance;
        LoadResources();
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;

        //Added 2020-05-21
        playerState = PlayerState.NOTSPOTTED;
        playerIsCaught = false;
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
        if(playerIsCaught)
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

    void LoadResources()
    {
        guardCameraVariables = Resources.Load<GuardCameraVariables>("References/Camera/StaticGuardCameraVariables");
        playerCamerasVariables = Resources.Load<PlayerCamerasVariables>("References/Camera/StaticPlayerCamerasVariables");
        guardDisabledSubject = Resources.Load<GuardDisabledSubject>("References/Guard/StaticGuardDisabledSubject");
        guardHackedSubject = Resources.Load<GuardHackedSubject>("References/Guard/StaticGuardHackedSubject");
        playerSoundSubject = Resources.Load<PlayerSoundSubject>("References/Player/StaticPlayerSoundSubject");
        playerSpottedSubject = Resources.Load<PlayerSpottedSubject>("References/Player/StaticPlayerSpottedSubject");
        interactionButtonSubject = Resources.Load<InteractionButtonSubject>("References/UI/StaticInteractionButtonSubject");
        settings = Resources.Load<Settings>("References/Settings/StaticSettings");
        audioManager = FindObjectOfType<AudioManager>();
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
