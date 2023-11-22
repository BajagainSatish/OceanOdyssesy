using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDisable : MonoBehaviour
{
    [SerializeField] private float projectileLifetime;
    private void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(DeactivateProjectile());
        }
    }
    private IEnumerator DeactivateProjectile()
    {
        yield return new WaitForSeconds(projectileLifetime);
        this.gameObject.SetActive(false);
    }
}
