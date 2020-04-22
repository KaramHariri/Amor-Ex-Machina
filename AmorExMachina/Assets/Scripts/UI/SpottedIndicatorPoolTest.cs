using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpottedIndicatorPoolTest : MonoBehaviour
{
    [SerializeField]
    private SpottedIndicatorTest indicatorPrefab = null;
    [SerializeField]
    private int numberOfIndicators = 10;

    public static SpottedIndicatorPoolTest instance = null;
    public List<SpottedIndicatorTest> pooledIndicators = null;
    [SerializeField] private RectTransform holder = null;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        pooledIndicators = new List<SpottedIndicatorTest>();
        for(int i = 0; i < numberOfIndicators; i++)
        {
            SpottedIndicatorTest indicator = Instantiate(indicatorPrefab, holder);
            indicator.gameObject.SetActive(false);
            pooledIndicators.Add(indicator);
        }
    }

    public SpottedIndicatorTest GetIndicator()
    {
        for(int i = 0; i < pooledIndicators.Count; i++)
        {
            if(!pooledIndicators[i].gameObject.activeInHierarchy)
            {
                return pooledIndicators[i];
            }
        }

        SpottedIndicatorTest indicator = Instantiate(indicatorPrefab, holder);
        indicator.gameObject.SetActive(false);
        pooledIndicators.Add(indicator);
        return indicator;
    }
}
