using Boo.Lang;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
    private PlayerState playerState = PlayerState.NOTSPOTTED;
    static public GameState previousState;

    private EventSystem eventSystem = null;

    public static Guard[] guards;

    private AudioManager audioManager = null;
    [SerializeField] private PlayerVariables playerVariables = null;

    private void Awake()
    {
        FindAllGuards();
        currentState = GameState.NORMALGAME;
        previousState = currentState;
        audioManager = FindObjectOfType<AudioManager>();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Options") && currentState != GameState.MENU)
        {
            StartCoroutine("SetPauseMenuSelectedButton");
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            currentState = GameState.MENU;
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

}
