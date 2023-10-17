using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float arrowLifetime;
    private void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            StartCoroutine(DeactivateArrow());
        }
    }
    private IEnumerator DeactivateArrow()
    {
        yield return new WaitForSeconds(arrowLifetime);
        this.gameObject.SetActive(false);
    }
}
