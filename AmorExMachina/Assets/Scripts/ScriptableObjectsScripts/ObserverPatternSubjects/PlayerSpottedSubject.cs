using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSpottedSubject",menuName = "Player/ PlayerSpottedSubject", order = 51)]
public class PlayerSpottedSubject : ScriptableObject
{
    public List<IPlayerSpottedObserver> observers = new List<IPlayerSpottedObserver>();

    public void AddObserver(IPlayerSpottedObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IPlayerSpottedObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(Vector3 position)
    {
        foreach (IPlayerSpottedObserver observer in observers)
        {
            observer.Notify(position);
        }
    }
}
