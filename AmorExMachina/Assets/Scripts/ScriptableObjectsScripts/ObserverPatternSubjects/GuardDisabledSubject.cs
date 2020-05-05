using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GuardDisabledSubject", menuName = "Guard/GuardDisabledSubject", order = 51)]
public class GuardDisabledSubject : ScriptableObject
{
    public List<IGuardDisabledObserver> observers = new List<IGuardDisabledObserver>();

    public void AddObserver(IGuardDisabledObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IGuardDisabledObserver observer)
    {
        observers.Remove(observer);
    }

    public void GuardDisabledNotify(Guard disabledGuardScript, bool isDisabled, bool isHacked)
    {
        foreach (IGuardDisabledObserver observer in observers)
        {
            observer.GuardDisabledNotify(disabledGuardScript, isDisabled, isHacked);
        }
    }
}
