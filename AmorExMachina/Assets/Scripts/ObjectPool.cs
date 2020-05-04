﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolItem
{
    public GameObject prefab;
    public int numberOfObjects;
    public bool expandable;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public List<PoolItem> items;
    public List<GameObject> pooledItems;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pooledItems = new List<GameObject>();
        foreach(PoolItem item in items)
        {
            for(int i = 0; i < item.numberOfObjects; i++)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                pooledItems.Add(obj);
            }
        }
    }

    public GameObject Get(string tag)
    {
        for(int i = 0; i < pooledItems.Count; i++)
        {
            if(!pooledItems[i].activeInHierarchy && pooledItems[i].tag == tag)
            {
                return pooledItems[i];
            }
        }

        foreach(PoolItem item in items)
        {
            if(item.prefab.tag == tag && item.expandable)
            {
                GameObject obj = Instantiate(item.prefab);
                obj.SetActive(false);
                pooledItems.Add(obj);
                return obj;
            }
        }

        return null;
    }
}
