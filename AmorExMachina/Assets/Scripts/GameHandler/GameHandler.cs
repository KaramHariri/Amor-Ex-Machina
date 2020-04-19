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
public class GameHandler : MonoBehaviour
{
    static public GameState currentState;
    private GameState previousState;

    public static Guard[] guards;

    private void Awake()
    {
        FindAllGuards();
        previousState = currentState;
    }

    void Update()
    {
        if(Input.GetButtonDown("Options"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if(previousState != currentState)
        {
            //Debug.Log("Switching state from :" + previousState + " to " + currentState);
            previousState = currentState;
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
}
