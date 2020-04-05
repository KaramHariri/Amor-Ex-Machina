using UnityEngine;

public class GameController : MonoBehaviour, IGameStateObserver
{
    public static Guard[] guards;
    public GameStateSubject gameStateSubject;

    private void Awake()
    {
        gameStateSubject.AddObserver(this);
        FindAllGuards();
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

    public void GameStateNotify(GameState gameState)
    {
        if (gameState == GameState.LOST)
        {
            // Lost.
        }
        else if (gameState == GameState.WON)
        {
            // Won.
        }
    }
}
