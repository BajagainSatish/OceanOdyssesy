using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    private int maxHealth;
    public int currentHealth;

    private Healthbar healthbarScript;
    private ShipCategorizer_Level shipCategorizer_LevelScript;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.name == "Canvas")
            {
                GameObject canvasGameObject = transform.GetChild(i).gameObject;
                GameObject healthbarGameObject = canvasGameObject.transform.GetChild(0).gameObject;

                healthbarScript = healthbarGameObject.GetComponent<Healthbar>();
            }
        }
    }
    private void Start()
    {
        if (!TryGetComponent<ShipCategorizer_Level>(out _))
        {
            maxHealth = SetParameters.supplyShipHealth;
        }
        else
        {
            shipCategorizer_LevelScript = GetComponent<ShipCategorizer_Level>();
            maxHealth = shipCategorizer_LevelScript.shipHealth;
        }

        currentHealth = maxHealth;
        healthbarScript.SetMaxHealth(maxHealth);
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthbarScript.SetHealth(currentHealth);
    }
}
