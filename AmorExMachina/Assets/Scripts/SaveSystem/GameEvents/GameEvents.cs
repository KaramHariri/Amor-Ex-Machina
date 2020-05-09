using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        if (current == null)
        {
            current = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public event Action onSaveDataEvent;
    public void SaveDataEvent()
    {
        if(onSaveDataEvent != null)
        {
            onSaveDataEvent();
        }
    }

    public event Action onLoadDataEvent;
    public void LoadDataEvent()
    {
        if (onLoadDataEvent != null)
        {
            onLoadDataEvent();
        }
    }
}
