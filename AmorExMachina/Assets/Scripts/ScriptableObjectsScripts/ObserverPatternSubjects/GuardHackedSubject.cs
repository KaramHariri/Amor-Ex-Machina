using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerHackedSubject", menuName = "Player/PlayerHackedSubject", order = 53)]
public class GuardHackedSubject : ScriptableObject
{
    public List<IGuardHackedObserver> observers = new List<IGuardHackedObserver>();

    public void AddObserver(IGuardHackedObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IGuardHackedObserver observer)
    {
        observers.Remove(observer);
    }

    public void GuardHackedNotify(string guardName)
    {
        foreach (IGuardHackedObserver observer in observers)
        {
            observer.GuardHackedNotify(guardName);
        }
    }
}
