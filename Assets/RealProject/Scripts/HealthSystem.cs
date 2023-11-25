using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    private int maxHealth;
    public int currentHealth;

    private Healthbar healthbarScript;
    private ShipCategorizer_Level shipCategorizer_LevelScript;
    private GameObject scaleFactorGameObject;
    private GameObject shooters;

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
            else if (transform.GetChild(i).gameObject.name == "ScaleFactorGameObject")
            {
                scaleFactorGameObject = transform.GetChild(i).gameObject;
            }
        }
        if (scaleFactorGameObject != null)
        {
            for (int i = 0; i < scaleFactorGameObject.transform.childCount; i++)
            {
                GameObject gameObject = scaleFactorGameObject.transform.GetChild(i).gameObject;
                if (gameObject.name == "Archers" || gameObject.name == "Gunmen" || gameObject.name == "CannonUnit" || gameObject.name == "MortarUnit")
                {
                    shooters = gameObject;
                }
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
    private void Update()
    {
        if (shooters != null)
        {
            DeactivateMen();
        }
    }
    private void DeactivateMen()
    {
        //Later Implement checks only when necessary rathen than including in Update() method to improve performance
        if (currentHealth <= 0)
        {
            shooters.SetActive(false);
        }
        else
        {
            shooters.SetActive(true);
        }
    }
}
