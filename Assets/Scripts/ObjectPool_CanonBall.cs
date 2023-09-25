using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool_CanonBall : MonoBehaviour
{
    [SerializeField] private GameObject canonBallPrefab;
    [SerializeField] private int totalCanonBallCount = 1;
    public bool noCannonBallSelectedYet;

    private void Start()
    {
        for (int i = 0; i < totalCanonBallCount; i++)
        {
            GameObject canonBall = Instantiate(canonBallPrefab,transform);
            canonBall.SetActive(false);
        }
        noCannonBallSelectedYet = true;
    }

    public GameObject ReturnCanonBall()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (!this.transform.GetChild(i).gameObject.activeInHierarchy && noCannonBallSelectedYet)
            {
                noCannonBallSelectedYet = false;
                return this.transform.GetChild(i).gameObject;
            }
        }
        return null;
    }
}
