using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShipCategorizer_Level;

public class CannonShoot : MonoBehaviour
{
    private GameObject scaleFactorGameObject;
    private GameObject cannonUnit;

    private readonly GameObject[] shootUnitCannon = new GameObject[SetParameters.mediumShipMenCount];

    private readonly CannonController[] cannonControllerScript = new CannonController[SetParameters.mediumShipMenCount];

    public int totalAmmoCount;
    private bool sufficientAmmoPresent;

    private ShipCategorizer_Level shipCategorizer_LevelScript;

    public Transform targetEnemy;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gameObject = transform.GetChild(i).gameObject;
            if (gameObject.name == "ScaleFactorGameObject")
            {
                scaleFactorGameObject = gameObject;
            }
        }
        for (int i = 0; i < scaleFactorGameObject.transform.childCount; i++)
        {
            GameObject gameObject = scaleFactorGameObject.transform.GetChild(i).gameObject;
            if (gameObject.name == "CannonUnit")
            {
                cannonUnit = gameObject;
            }
        }
        for (int i = 0; i < SetParameters.mediumShipMenCount; i++)
        {
            shootUnitCannon[i] = cannonUnit.transform.GetChild(i).gameObject;
            cannonControllerScript[i] = shootUnitCannon[i].GetComponent<CannonController>();
        }
    }
    private void Start()
    {
        sufficientAmmoPresent = true;
        shipCategorizer_LevelScript = GetComponent<ShipCategorizer_Level>();

        if (shipCategorizer_LevelScript.shipLevel == ShipLevels.Level1)
        {
            AssignValue(0);
        }
        else if (shipCategorizer_LevelScript.shipLevel == ShipLevels.Level2)
        {
            AssignValue(1);
        }
        else if (shipCategorizer_LevelScript.shipLevel == ShipLevels.Level3)
        {
            AssignValue(2);
        }
        else if (shipCategorizer_LevelScript.shipLevel == ShipLevels.Level4)
        {
            AssignValue(3);
        }
        targetEnemy = null;
    }
    private void Update()
    {
        HandleAmmoCount();
        //Cannon Controller
        for (int i = 0; i < SetParameters.mediumShipMenCount; i++)
        {
            if (!sufficientAmmoPresent)
            {
                cannonControllerScript[i].enableLineRenderer = false;//during experimentation, showed linerenderer at previous path points
            }

        }
    }
    private void HandleAmmoCount()
    {
        if (totalAmmoCount <= 0)
        {
            sufficientAmmoPresent = false;
        }
        else
        {
            sufficientAmmoPresent = true;
        }
    }
    private void AssignValue(int index)
    {
        totalAmmoCount = SetParameters.archerWeaponMaxAmmo[index];
    }
}

//Other functional portion in respective CannonController script
/*
            if (hasNotShotEvenOnce)
            {
                gunmanControllerScript[i].enableLineRenderer = true;
            }
            if (Input.GetKeyDown(KeyCode.S))//shoot only if ship is selected
            {
                hasNotShotEvenOnce = false;
            }
 */