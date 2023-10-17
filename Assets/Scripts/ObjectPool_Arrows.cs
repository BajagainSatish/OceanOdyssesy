using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_Arrows : MonoBehaviour
{
    [SerializeField] private int totalArrowCount = 20;
    [SerializeField] private GameObject arrowPrefab;

    private void Awake()
    {
        for (int i = 0; i < totalArrowCount; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform);
            arrow.SetActive(false);
        }
    }

    public GameObject ReturnArrow()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject child = this.transform.GetChild(i).gameObject;
            if (!child.activeInHierarchy)
            {
                child.SetActive(true);
                return child;
            }
        }

        // If no inactive objects are available, instantiate a new one
        GameObject newArrow = Instantiate(arrowPrefab);
        newArrow.transform.SetParent(this.transform); // Add it to the pool
        newArrow.SetActive(true);
        return newArrow;
    }
}
