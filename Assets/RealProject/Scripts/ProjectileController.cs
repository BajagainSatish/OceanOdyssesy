using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public int weaponDamage;
    public bool isPlayer1Projectile;

    /*private GameObject sparks;
    private ParticleSystem sparkParticleSystem;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Sparks")
            {
                sparks = transform.GetChild(i).gameObject;
            }
        }
    }
    private void Start()
    {
        sparkParticleSystem = sparks.GetComponent<ParticleSystem>();
    }
    private void Update()
    {
        if (this.gameObject.activeInHierarchy)
        {
            sparkParticleSystem.Play();
        }
    }*/
}
