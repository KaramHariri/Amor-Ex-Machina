using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameStateSubject", menuName = "GameState/ GameStateSubject", order = 50)]
public class GameStateSubject : ScriptableObject
{
    public List<IGameStateObserver> observers = new List<IGameStateObserver>();

    public void AddObserver(IGameStateObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IGameStateObserver observer)
    {
        observers.Remove(observer);
    }

    public void GameStateNotify(GameState gameState)
    {
        foreach (IGameStateObserver observer in observers)
        {
            observer.GameStateNotify(gameState);
        }
    }
}