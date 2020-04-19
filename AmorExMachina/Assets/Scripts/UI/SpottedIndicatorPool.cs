using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpottedIndicatorPool : MonoBehaviour
{
    [SerializeField]
    private SpottedIndicator indicatorPrefab = null;
    [SerializeField]
    private int numberOfIndicators = 10;

    public static SpottedIndicatorPool instance = null;
    public List<SpottedIndicator> pooledIndicators = null;
    [SerializeField] private RectTransform holder = null;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pooledIndicators = new List<SpottedIndicator>();
        for(int i = 0; i < numberOfIndicators; i++)
        {
            SpottedIndicator indicator = Instantiate(indicatorPrefab, holder);
            indicator.gameObject.SetActive(false);
            pooledIndicators.Add(indicator);
        }
    }

    public SpottedIndicator GetIndicator()
    {
        for(int i = 0; i < pooledIndicators.Count; i++)
        {
            if(!pooledIndicators[i].gameObject.activeInHierarchy)
            {
                return pooledIndicators[i];
            }
        }

        SpottedIndicator indicator = Instantiate(indicatorPrefab, holder);
        indicator.gameObject.SetActive(false);
        pooledIndicators.Add(indicator);
        return indicator;
    }
}
