using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetingSystem_PhysicsOverlapSphere : MonoBehaviour
{
    private ShipCategorizer_Player thisShipCategorizerPlayerScript;
    private bool isPlayer1;
    private GameObject target;
    private Collider targetCollider;

    private GameObject scaleFactorGameObject;
    private GameObject parentShooterObject;
    private string shooter;

    private readonly GameObject[] shooters = new GameObject[SetParameters.mediumShipMenCount];
    private readonly ArcherController[] archerControllerScript = new ArcherController[SetParameters.mediumShipMenCount];
    private readonly CannonController[] cannonControllerScript = new CannonController[SetParameters.mediumShipMenCount];
    private readonly GunmanController[] gunmanControllerScript = new GunmanController[SetParameters.mediumShipMenCount];
    private readonly MortarController[] mortarControllerScript = new MortarController[SetParameters.mediumShipMenCount];

    private Transform shipCenter;
    //public bool testActiveShip;
    private float shipMaxRange;

    private List<Collider> enemyShipsInRange = new List<Collider>();

    public enum ShipType
    {
        ArcherShip, CannonShip, GunmanShip, MortarShip, SupplyShip
    };
    public ShipType thisShipType;
    private Vector3 myShipPosition;
    private bool thisShipIsFunctional;

    private ShipCategorizer_Level thisShipCategorizer_LevelScript;

    private void Awake()
    {
        thisShipCategorizer_LevelScript = GetComponent<ShipCategorizer_Level>();

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
            thisShipType = ShipType.ArcherShip;
        }
        else if (TryGetComponent<CannonShoot>(out _))
        {
            thisShipType = ShipType.CannonShip;
        }
        else if (TryGetComponent<GunShoot>(out _))
        {
            thisShipType = ShipType.GunmanShip;
        }
        else if (TryGetComponent<MortarShoot>(out _))
        {
            thisShipType = ShipType.MortarShip;
        }
        else
        {
            thisShipType = ShipType.SupplyShip;
        }

        shipCenter = transform.GetChild(0).transform;
    }
    private void Start()
    {
        thisShipCategorizerPlayerScript = GetComponent<ShipCategorizer_Player>();
        isPlayer1 = thisShipCategorizerPlayerScript.isP1Ship;
        shipMaxRange = thisShipCategorizer_LevelScript.weaponRange;
    }

    private void Update()
    {
        myShipPosition = shipCenter.position;

        thisShipIsFunctional = thisShipCategorizerPlayerScript.isFunctionalShip;
        if (thisShipIsFunctional)
        {
            AddEnemyShipsInRangeToOurList();
            RemoveShipsOutsideRangeFromOurList();
            DetermineWhichShipToAttack();
        }
        else
        {
            enemyShipsInRange.Clear();
            target = null;
        }
        AssignTargetToEachAttackerShip();
        //TestShipCode();
    }

    private void AddEnemyShipsInRangeToOurList()
    {
        Collider[] colliderArray = Physics.OverlapSphere(shipCenter.position, shipMaxRange);

        // Create a copy of the original list to avoid modification during iteration
        List<Collider> tempList = new List<Collider>(enemyShipsInRange);

        foreach (Collider collider in colliderArray)
        {
            if (collider.TryGetComponent<ShipCategorizer_Player>(out _))
            {
                ShipCategorizer_Player shipCategorizer_PlayerScript = collider.GetComponent<ShipCategorizer_Player>();
                bool shipInRangeIsPlayer1 = shipCategorizer_PlayerScript.isP1Ship;
                bool shipInRangeIsFunctional = shipCategorizer_PlayerScript.isFunctionalShip;

                if (shipInRangeIsPlayer1 != isPlayer1 && shipInRangeIsFunctional)
                {
                    Vector3 enemyShipPosition = collider.transform.GetChild(0).transform.position;

                    // Calculate the distance between this ship and the current enemy ship
                    float distance = Vector3.Distance(myShipPosition, enemyShipPosition);

                    if (distance <= shipMaxRange)
                    {
                        if (!tempList.Contains(collider))
                        {
                            /*if (testActiveShip)
                            {
                                print("Added " + collider.name + " to our list.");
                            }*/
                            enemyShipsInRange.Add(collider);
                        }
                    }  
                }
            }
        }
    }
    private void RemoveShipsOutsideRangeFromOurList()
    {
        // Create a copy of the original list to avoid modification during iteration
        List<Collider> tempEnemyShipsInRangeList = new List<Collider>(enemyShipsInRange);

        foreach (Collider enemyShip in tempEnemyShipsInRangeList)
        {
            Vector3 enemyShipPosition = enemyShip.transform.GetChild(0).transform.position;

            // Calculate the distance between this ship and the current enemy ship
            float distance = Vector3.Distance(myShipPosition, enemyShipPosition);

            ShipCategorizer_Player shipCategorizer_PlayerScript = enemyShip.GetComponent<ShipCategorizer_Player>();
            bool shipInRangeIsFunctional = shipCategorizer_PlayerScript.isFunctionalShip;

            if (distance > shipMaxRange || !shipInRangeIsFunctional)
            {
                /*if (testActiveShip)
                {
                    print("Removed " + enemyShip.name + " from our list.");
                }*/
                enemyShipsInRange.Remove(enemyShip);
            }              
        }
    }
    private void DetermineWhichShipToAttack()
    {
        NoTargetIfNoShipInRange();
        OneShipInRangeCase();
        MultipleShipsInRange();
    }
    private void NoTargetIfNoShipInRange()
    {
        if (enemyShipsInRange.Count == 0)
        {
            target = null;
            targetCollider = null;
        }
    }
    private void OneShipInRangeCase()
    {
        if (enemyShipsInRange.Count == 1)
        {
            foreach (Collider oneEnemyShipInList in enemyShipsInRange)
            {
                targetCollider = oneEnemyShipInList;
                target = targetCollider.gameObject;//ShipCenter of enemyShip is target
            }
        }
    }
    private void MultipleShipsInRange()
    {
        if (enemyShipsInRange.Count > 1)
        {
            if (target == null)
            {
                SelectAnotherShipInRangeAsTarget();
            }
            IfTargetMovesOutsideOfRange();
        }
    }
    private void SelectAnotherShipInRangeAsTarget()
    {
        if (enemyShipsInRange.Count > 1)
        {
            // Keep track of the nearest ship and its distance
            Collider nearestEnemyShip = null;
            float nearestDistance = float.MaxValue;//Initially set to very large arbitrary value so that initial comparison in first iteration is always true.

            //Find nearmost collider
            foreach (Collider enemyShip in enemyShipsInRange)
            {
                Vector3 enemyShipPosition = enemyShip.transform.GetChild(0).transform.position;

                // Calculate the distance between this ship and the current enemy ship
                float distance = Vector3.Distance(myShipPosition, enemyShipPosition);

                // Check if this is the closest ship so far
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemyShip = enemyShip;
                }
            }

            //Finally assign target
            if (nearestEnemyShip != null)
            {
                target = nearestEnemyShip.gameObject;
            }
        }
    }
    private void IfTargetMovesOutsideOfRange()
    {
        //Check for presence of target inside range. If target goes outside range, select another nearmost ship as target.

        // Create a copy of the original list to avoid modification during iteration
        List<Collider> tempEnemyShipsInRangeList = new List<Collider>(enemyShipsInRange);

        if (targetCollider != null)
        {
            //If our target ship is not inside enemyShipsInRange List, select another ship
            if (!tempEnemyShipsInRangeList.Contains(targetCollider))
            {
                SelectAnotherShipInRangeAsTarget();
            }
        }       
    }
    private void AssignTargetToEachAttackerShip()
    {
        if (target != null)
        {
            GameObject targetShipCenter = target.transform.GetChild(0).gameObject;
            Transform targetShipCenterTransform = targetShipCenter.transform;

            if (thisShipType == ShipType.ArcherShip)
            {
                foreach (ArcherController subArcherControllerScript in archerControllerScript)
                {
                    subArcherControllerScript.B = targetShipCenterTransform;
                }
            }
            else if (thisShipType == ShipType.CannonShip)
            {
                foreach (CannonController subCannonControllerScript in cannonControllerScript)
                {
                    subCannonControllerScript.B = targetShipCenterTransform;
                }
            }
            else if (thisShipType == ShipType.GunmanShip)
            {
                foreach (GunmanController subGunmanControllerScript in gunmanControllerScript)
                {
                    subGunmanControllerScript.B = targetShipCenterTransform;
                }
            }
            else if (thisShipType == ShipType.MortarShip)
            {
                foreach (MortarController subMortarControllerScript in mortarControllerScript)
                {
                    subMortarControllerScript.B = targetShipCenterTransform;
                }
            }
        }
        else
        {
            if (thisShipType == ShipType.ArcherShip)
            {
                foreach (ArcherController subArcherControllerScript in archerControllerScript)
                {
                    subArcherControllerScript.B = null;
                }
            }
            else if (thisShipType == ShipType.CannonShip)
            {
                foreach (CannonController subCannonControllerScript in cannonControllerScript)
                {
                    subCannonControllerScript.B = null;
                }
            }
            else if (thisShipType == ShipType.GunmanShip)
            {
                foreach (GunmanController subGunmanControllerScript in gunmanControllerScript)
                {
                    subGunmanControllerScript.B = null;
                }
            }
            else if (thisShipType == ShipType.MortarShip)
            {
                foreach (MortarController subMortarControllerScript in mortarControllerScript)
                {
                    subMortarControllerScript.B = null;
                }
            }
        }
    }
    /*private void TestShipCode()
    {
        if (testActiveShip)
        {
            foreach (Collider enemyShip in enemyShipsInRange)
            {
                print(enemyShip.name);
            }
            if (target != null)
            {
                print("Target: " + target.GetComponentInParent<Collider>().name);
            }
        }
    }*/
}

//Use of the method Physics.OverlapSphere() also adds current ship into the array. The length of array increases as number of ships in range increases and decreases accordingly.
//By default, if no any ships are in range, array length is 1(this ship's collider is included). Other colliders, eg. Base Ground if in range are also added that may increase array length.
//If there are 3 ships in range, array length should be >= 5(Array Length) = 3(ships in range) + 1(ship with this script which also has collider) + 1(Other colliders, eg.Base Ground), 1 nearyby island, etc.