using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonArtilleryShoot : MonoBehaviour
{
    private GameObject scaleFactorGameObject;
    private GameObject cannonUnit;
    private GameObject mortarUnit;

    private readonly GameObject[] shootUnitCannon = new GameObject[CannonController.totalCannonCount];
    private readonly GameObject[] shootUnitMortar = new GameObject[MortarController.totalMortarCount];

    private GameObject[] mortarObject = new GameObject[MortarController.totalMortarCount];
    private GameObject[] mortarBarrel = new GameObject[MortarController.totalMortarCount];

    private readonly CannonController[] cannonControllerScript = new CannonController[CannonController.totalCannonCount];
    private readonly MortarController[] mortarControllerScript = new MortarController[MortarController.totalMortarCount];

    private ShipClassifier shipClassifierScript;

    public bool shipIsActive;
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
            else if (gameObject.name == "MortarUnit")
            {
                mortarUnit = gameObject;
            }
        }
        for (int i = 0; i < CannonController.totalCannonCount; i++)
        {
            shootUnitCannon[i] = cannonUnit.transform.GetChild(i).gameObject;
            cannonControllerScript[i] = shootUnitCannon[i].GetComponent<CannonController>();
        }
        for (int i = 0; i < MortarController.totalMortarCount; i++)
        {
            shootUnitMortar[i] = mortarUnit.transform.GetChild(i).gameObject;
            mortarControllerScript[i] = shootUnitMortar[i].GetComponent<MortarController>();
        }
        shipClassifierScript = GetComponent<ShipClassifier>();

        for (int i = 0; i < MortarController.totalMortarCount; i++)
        {
            mortarObject[i] = shootUnitMortar[i].transform.GetChild(0).gameObject;
            mortarBarrel[i] = mortarObject[i].transform.GetChild(0).gameObject;
        }
    }

    private void Start()
    {
        shipIsActive = false;
        hasNotShotEvenOnce = true;
    }

    private void Update()
    {
        shipIsActive = shipClassifierScript.isActive;

        //Cannon Controller
        for (int i = 0; i < CannonController.totalCannonCount; i++)
        {
            //Show line renderer only for currently selected ship, and if not during cool down time
            if (shipIsActive && cannonControllerScript[i].enableLineRenderer)
            {
                cannonControllerScript[i].enableLineRenderer = true;
            }
            else if (!shipIsActive)
            {
                cannonControllerScript[i].enableLineRenderer = false;
            }
        }

        //Mortar Controller
        for (int i = 0; i < MortarController.totalMortarCount; i++)
        {
            //Show line renderer only for currently selected ship, and if not during cool down time
            if (shipIsActive && mortarControllerScript[i].enableLineRenderer)
            {
                mortarControllerScript[i].enableLineRenderer = true;
            }
            else if (!shipIsActive)
            {
                mortarControllerScript[i].enableLineRenderer = false;
            }

            Transform B = mortarControllerScript[i].B;
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
}

//Other functional portion in respective script
/*
            if (hasNotShotEvenOnce && shipIsActive)
            {
                gunmanControllerScript[i].enableLineRenderer = true;
            }
            if (Input.GetKeyDown(KeyCode.S) && shipIsActive)//shoot only if ship is selected
            {
                hasNotShotEvenOnce = false;
            }
 */