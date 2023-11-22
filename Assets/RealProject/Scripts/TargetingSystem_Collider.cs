using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem_Collider : MonoBehaviour
{
    public static int smallShipMenCount = 2;
    public static int mediumShipMenCount = 4;
    public static int largeShipMenCount = 6;

    private ShipCategorizer_Player shipCategorizerPlayerScript;

    private bool isPlayer1;
    private GameObject target;

    private GameObject scaleFactorGameObject;
    private GameObject parentShooterObject;
    private string shooter;

    private readonly GameObject[] shooters = new GameObject[mediumShipMenCount];

    private readonly ArcherController[] archerControllerScript = new ArcherController[mediumShipMenCount];
    private readonly CannonController[] cannonControllerScript = new CannonController[mediumShipMenCount];
    private readonly GunmanController[] gunmanControllerScript = new GunmanController[mediumShipMenCount];
    private readonly MortarController[] mortarControllerScript = new MortarController[mediumShipMenCount];

    public bool selectedTemporarilyForExperimentation;

    public enum ShipType
    {
        ArcherShip, CannonShip, GunmanShip, MortarShip, SupplyShip
    };
    public ShipType shipType;

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
            if (gameObject.name == "Archers" || gameObject.name == "Gunmen" || gameObject.name == "CannonUnit" || gameObject.name == "MortarUnit")
            {
                parentShooterObject = gameObject;
                shooter = parentShooterObject.name;
            }
        }
        if (parentShooterObject != null)
        {
            for (int i = 0; i < parentShooterObject.transform.childCount; i++)
            {
                if (shooter == "Archers")
                {
                    shooters[i] = parentShooterObject.transform.GetChild(i).gameObject;
                    archerControllerScript[i] = shooters[i].GetComponent<ArcherController>();
                }
                else if (shooter == "CannonUnit")
                {
                    shooters[i] = parentShooterObject.transform.GetChild(i).gameObject;
                    cannonControllerScript[i] = shooters[i].GetComponent<CannonController>();
                }
                else if (shooter == "Gunmen")
                {
                    shooters[i] = parentShooterObject.transform.GetChild(i).gameObject;
                    gunmanControllerScript[i] = shooters[i].GetComponent<GunmanController>();
                }
                else if (shooter == "MortarUnit")
                {
                    shooters[i] = parentShooterObject.transform.GetChild(i).gameObject;
                    mortarControllerScript[i] = shooters[i].GetComponent<MortarController>();
                }
            }
        }

        if (TryGetComponent<ArcherShoot>(out _))
        {
            shipType = ShipType.ArcherShip;
        }
        else if (TryGetComponent<CannonShoot>(out _))
        {
            shipType = ShipType.CannonShip;
        }
        else if (TryGetComponent<GunShoot>(out _))
        {
            shipType = ShipType.GunmanShip;
        }
        else if (TryGetComponent<MortarShoot>(out _))
        {
            shipType = ShipType.MortarShip;
        }
        else
        {
            shipType = ShipType.SupplyShip;
        }
    }

    private void Start()
    {
        shipCategorizerPlayerScript = GetComponent<ShipCategorizer_Player>();
        isPlayer1 = shipCategorizerPlayerScript.isP1Ship;
    }

    private void Update()
    {
        if (target != null)
        {
            if (shipType == ShipType.ArcherShip)
            {
                foreach (ArcherController subArcherControllerScript in archerControllerScript)
                {
                    subArcherControllerScript.B = target.transform;
                }
            }
            else if (shipType == ShipType.CannonShip)
            {
                foreach (CannonController subCannonControllerScript in cannonControllerScript)
                {
                    subCannonControllerScript.B = target.transform;
                }
            }
            else if (shipType == ShipType.GunmanShip)
            {
                foreach (GunmanController subGunmanControllerScript in gunmanControllerScript)
                {
                    subGunmanControllerScript.B = target.transform;
                }
            }
            else if (shipType == ShipType.MortarShip)
            {
                foreach (MortarController subMortarControllerScript in mortarControllerScript)
                {
                    subMortarControllerScript.B = target.transform;
                }
            }
        }
        else
        {
            if (shipType == ShipType.ArcherShip)
            {
                foreach (ArcherController subArcherControllerScript in archerControllerScript)
                {
                    subArcherControllerScript.B = null;
                }
            }
            else if (shipType == ShipType.CannonShip)
            {
                foreach (CannonController subCannonControllerScript in cannonControllerScript)
                {
                    subCannonControllerScript.B = null;
                }
            }
            else if (shipType == ShipType.GunmanShip)
            {
                foreach (GunmanController subGunmanControllerScript in gunmanControllerScript)
                {
                    subGunmanControllerScript.B = null;
                }
            }
            else if (shipType == ShipType.MortarShip)
            {
                foreach (MortarController subMortarControllerScript in mortarControllerScript)
                {
                    subMortarControllerScript.B = null;
                }
            }
        }

        if (selectedTemporarilyForExperimentation)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                if (target != null)
                {
                    print("Target: " + target);
                }
                else
                {
                    print("Target null");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.TryGetComponent<ShipCategorizer_Player>(out ShipCategorizer_Player anotherShip))
        {
            if (isPlayer1 != anotherShip.isP1Ship)//means one of the ships is player1 ship, and the other is player2 ship
            {
                print("Enemy in Range, will begin attack");
                target = collision.transform.GetChild(0).gameObject;//ShipCenter for now
            }
            else if(isPlayer1 == anotherShip.isP1Ship)
            {
                print("Friend ship in range");
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.TryGetComponent<ShipCategorizer_Player>(out ShipCategorizer_Player anotherShip))
        {
            if (isPlayer1 != anotherShip.isP1Ship)//means one of the ships is player1 ship, and the other is player2 ship
            {
                print("Enemy out of Range, will cease attack");
                target = null;//ShipCenter for now
            }
            else if (isPlayer1 == anotherShip.isP1Ship)
            {
                print("Friend ship out of range");
            }
        }
    }
}
