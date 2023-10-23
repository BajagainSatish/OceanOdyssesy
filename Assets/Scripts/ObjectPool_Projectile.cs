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
        GameObject newProjectile = Instantiate(projectilePrefab);
        newProjectile.transform.SetParent(this.transform); // Add it to the pool
        newProjectile.SetActive(true);
        return newProjectile;
    }
}
