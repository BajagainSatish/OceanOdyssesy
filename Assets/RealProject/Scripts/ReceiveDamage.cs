using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveDamage : MonoBehaviour
{
    private HealthSystem healthSystemScript;
    private ShipCategorizer_Player shipCategorizer_PlayerScript;

    private bool thisShipIsPlayer1;

    private void Awake()
    {
        healthSystemScript = GetComponent<HealthSystem>();
        shipCategorizer_PlayerScript = GetComponent<ShipCategorizer_Player>();
    }

    private void Start()
    {
        thisShipIsPlayer1 = shipCategorizer_PlayerScript.isP1Ship;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ProjectileController>(out ProjectileController projectileControllerScript))
        {
            bool projectileIsOfPlayer1 = projectileControllerScript.isPlayer1Projectile;
            if (thisShipIsPlayer1 != projectileIsOfPlayer1)
            {
                int damage = projectileControllerScript.weaponDamage;
                healthSystemScript.TakeDamage(damage);
                print(this.name + " took damage: " + damage);
            }          
        }
    }
}
