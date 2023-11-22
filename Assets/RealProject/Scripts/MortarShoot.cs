using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarShoot : MonoBehaviour
{
    private GameObject scaleFactorGameObject;
    private GameObject mortarUnit;

    private readonly GameObject[] shootUnitMortar = new GameObject[MortarController.totalMortarCount];

    private GameObject[] mortarObject = new GameObject[MortarController.totalMortarCount];
    private GameObject[] mortarBarrel = new GameObject[MortarController.totalMortarCount];

    private readonly MortarController[] mortarControllerScript = new MortarController[MortarController.totalMortarCount];

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
            if (gameObject.name == "MortarUnit")
            {
                mortarUnit = gameObject;
            }
        }
        for (int i = 0; i < MortarController.totalMortarCount; i++)
        {
            shootUnitMortar[i] = mortarUnit.transform.GetChild(i).gameObject;
            mortarControllerScript[i] = shootUnitMortar[i].GetComponent<MortarController>();
        }

        for (int i = 0; i < MortarController.totalMortarCount; i++)
        {
            mortarObject[i] = shootUnitMortar[i].transform.GetChild(0).gameObject;
            mortarBarrel[i] = mortarObject[i].transform.GetChild(0).gameObject;
        }
    }

    private void Start()
    {
        hasNotShotEvenOnce = true;
    }

    private void Update()
    {
        //Mortar Controller
        for (int i = 0; i < MortarController.totalMortarCount; i++)
        {
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