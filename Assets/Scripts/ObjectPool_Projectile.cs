using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_Projectile : MonoBehaviour
{
    [SerializeField] private int totalProjectileCount = 50;
    [SerializeField] private GameObject projectilePrefab;

    private void Awake()
    {
        for (int i = 0; i < totalProjectileCount; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform);
            projectile.SetActive(false);
        }
    }

    public GameObject ReturnProjectile()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (!child.activeInHierarchy)
            {
                child.SetActive(true);
                return child;
            }
        }

        // If no inactive objects are available, instantiate a new one
        GameObject newProjectile = Instantiate(projectilePrefab);
        newProjectile.transform.SetParent(transform); // Add it to the pool
        newProjectile.SetActive(true);
        return newProjectile;
    }
}
