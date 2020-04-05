using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerLastSightPositionSubject",menuName = "Player/ PlayerLastSightPositionSubject", order = 51)]
public class PlayerLastSightPositionSubject : ScriptableObject
{
    public List<IPlayerLastSightPositionObserver> observers = new List<IPlayerLastSightPositionObserver>();

    public void AddObserver(IPlayerLastSightPositionObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IPlayerLastSightPositionObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(Vector3 position)
    {
        foreach (IPlayerLastSightPositionObserver observer in observers)
        {
            observer.Notify(position);
        }
    }
}
