using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShipCategorizer_Level;

public class MortarShoot : MonoBehaviour
{
    private GameObject scaleFactorGameObject;
    private GameObject mortarUnit;

    private readonly GameObject[] shootUnitMortar = new GameObject[SetParameters.mediumShipMenCount];

    private GameObject[] mortarObject = new GameObject[SetParameters.mediumShipMenCount];
    private GameObject[] mortarBarrel = new GameObject[SetParameters.mediumShipMenCount];

    private readonly MortarController[] mortarControllerScript = new MortarController[SetParameters.mediumShipMenCount];

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
            if (gameObject.name == "MortarUnit")
            {
                mortarUnit = gameObject;
            }
        }
        for (int i = 0; i < SetParameters.mediumShipMenCount; i++)
        {
            shootUnitMortar[i] = mortarUnit.transform.GetChild(i).gameObject;
            mortarControllerScript[i] = shootUnitMortar[i].GetComponent<MortarController>();
        }

        for (int i = 0; i < SetParameters.mediumShipMenCount; i++)
        {
            mortarObject[i] = shootUnitMortar[i].transform.GetChild(0).gameObject;
            mortarBarrel[i] = mortarObject[i].transform.GetChild(0).gameObject;
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
        //Mortar Controller
        for (int i = 0; i < SetParameters.mediumShipMenCount; i++)
        {
            Transform B = mortarControllerScript[i].B;
            if (!sufficientAmmoPresent)
            {
                mortarControllerScript[i].enableLineRenderer = false;//during experimentation, showed linerenderer at previous path points
            }
            if (B != null)
            {
                Vector3 targetDirection = (B.position - mortarBarrel[i].transform.position).normalized;
                mortarBarrel[i].transform.rotation = Quaternion.LookRotation(targetDirection);
                mortarBarrel[i].transform.localEulerAngles = new Vector3(0, mortarBarrel[i].transform.localEulerAngles.y, 0);
            }
            if (mortarBarrel != null && B != null)
            {
                Debug.DrawLine(mortarBarrel[i].transform.position, B.position, Color.blue);
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
//Other functional portion in respective Mortar Controller script
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
