using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShoot : MonoBehaviour
{
    private GameObject scaleFactorGameObject;
    private GameObject cannonUnit;

    private readonly GameObject[] shootUnitCannon = new GameObject[SetParameters.mediumShipMenCount];

    private readonly CannonController[] cannonControllerScript = new CannonController[SetParameters.mediumShipMenCount];

    public bool hasNotShotEvenOnce;//ensure that line renderer is visible at start if enemy ship is inside range, once visible it has no other significance

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
        hasNotShotEvenOnce = true;
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