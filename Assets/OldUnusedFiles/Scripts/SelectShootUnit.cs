using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectShootUnit : MonoBehaviour
{
    private GameObject shootUnitFLCharacter;
    private GameObject shootUnitBLCharacter;
    private GameObject shootUnitFRCharacter;
    private GameObject shootUnitBRCharacter;

    private AnimationStateController scriptForUnitFL;
    private AnimationStateController scriptForUnitBL;
    private AnimationStateController scriptForUnitFR;
    private AnimationStateController scriptForUnitBR;

    private void Awake()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).name == "ShootUnitFL")
            {
                GameObject shootUnitFL = this.transform.GetChild(i).gameObject;
                for (int j = 0; j < shootUnitFL.transform.childCount; j++)
                {
                    if (shootUnitFL.transform.GetChild(j).name == "Pirates")
                    {
                        GameObject pirates = shootUnitFL.transform.GetChild(j).gameObject;
                        shootUnitFLCharacter = pirates.transform.GetChild(0).gameObject;
                        scriptForUnitFL = shootUnitFLCharacter.GetComponent<AnimationStateController>();
                        pirates.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
            else if (this.transform.GetChild(i).name == "ShootUnitBL")
            {
                GameObject shootUnitBL = this.transform.GetChild(i).gameObject;
                for (int j = 0; j < shootUnitBL.transform.childCount; j++)
                {
                    if (shootUnitBL.transform.GetChild(j).name == "Pirates")
                    {
                        GameObject pirates = shootUnitBL.transform.GetChild(j).gameObject;
                        shootUnitBLCharacter = pirates.transform.GetChild(0).gameObject;
                        scriptForUnitBL = shootUnitBLCharacter.GetComponent<AnimationStateController>();
                        pirates.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
            else if (this.transform.GetChild(i).name == "ShootUnitFR")
            {
                GameObject shootUnitFR = this.transform.GetChild(i).gameObject;
                for (int j = 0; j < shootUnitFR.transform.childCount; j++)
                {
                    if (shootUnitFR.transform.GetChild(j).name == "Pirates")
                    {
                        GameObject pirates = shootUnitFR.transform.GetChild(j).gameObject;
                        shootUnitFRCharacter = pirates.transform.GetChild(0).gameObject;
                        scriptForUnitFR = shootUnitFRCharacter.GetComponent<AnimationStateController>();
                        pirates.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
            else if (this.transform.GetChild(i).name == "ShootUnitBR")
            {
                GameObject shootUnitBR = this.transform.GetChild(i).gameObject;
                for (int j = 0; j < shootUnitBR.transform.childCount; j++)
                {
                    if (shootUnitBR.transform.GetChild(j).name == "Pirates")
                    {
                        GameObject pirates = shootUnitBR.transform.GetChild(j).gameObject;
                        shootUnitBRCharacter = pirates.transform.GetChild(0).gameObject;
                        scriptForUnitBR = shootUnitBRCharacter.GetComponent<AnimationStateController>();
                        pirates.transform.GetChild(1).gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            scriptForUnitFL.selectedUnitToShoot = true;
        }
        else
        {
            scriptForUnitFL.selectedUnitToShoot = false;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            scriptForUnitBL.selectedUnitToShoot = true;
        }
        else
        {
            scriptForUnitBL.selectedUnitToShoot = false;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            scriptForUnitFR.selectedUnitToShoot = true;
        }
        else
        {
            scriptForUnitFR.selectedUnitToShoot = false;
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            scriptForUnitBR.selectedUnitToShoot = true;
        }
        else
        {
            scriptForUnitBR.selectedUnitToShoot = false;
        }
    }
}
