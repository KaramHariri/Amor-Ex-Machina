using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSoundSubject", menuName = "Player/ PlayerSoundSubject", order = 50)]
public class PlayerSoundSubject : ScriptableObject
{
    public List<IPlayerSoundObserver> observers = new List<IPlayerSoundObserver>();

    public void AddObserver(IPlayerSoundObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IPlayerSoundObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers(SoundType soundType, Vector3 soundPosition)
    {
        foreach (IPlayerSoundObserver observer in observers)
        {
            observer.PlayerSoundNotify(soundType, soundPosition);
        }
    }
}
