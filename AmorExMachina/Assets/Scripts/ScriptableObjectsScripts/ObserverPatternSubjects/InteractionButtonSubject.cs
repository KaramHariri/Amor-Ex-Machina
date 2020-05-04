using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionButtonSubject", menuName = "UI/InteractionButtonSubject", order = 51)]
public class InteractionButtonSubject : ScriptableObject
{
    public List<IInteractionButton> observers = new List<IInteractionButton>();

    public void AddObserver(IInteractionButton observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IInteractionButton observer)
    {
        observers.Remove(observer);
    }

    public void NotifyToShowInteractionButton(InteractionButtons buttonToShow)
    {
        foreach (IInteractionButton observer in observers)
        {
            observer.NotifyToShowInteractionButton(buttonToShow);
        }
    }

    public void NotifyToHideInteractionButton(InteractionButtons buttonToHide)
    {
        foreach(IInteractionButton observer in observers)
        {
            observer.NotifyToHideInteractionButton(buttonToHide);
        }
    }
}
