using Boo.Lang;
using UnityEngine;
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
    private GameState previousState;

    public static Guard[] guards;

    private AudioManager audioManager = null;
    [SerializeField] private PlayerVariables playerVariables = null;

    private void Awake()
    {
        FindAllGuards();
        currentState = GameState.NORMALGAME;
        previousState = currentState;
        audioManager = FindObjectOfType<AudioManager>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Options"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if(previousState != currentState)
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

}
