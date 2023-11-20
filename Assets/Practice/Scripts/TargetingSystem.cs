using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    private ShipClassifier[] shipsInRange = new ShipClassifier[ShipClassifier.shipCount];
    private ShipClassifier[] circularlyArrangedShips = new ShipClassifier[ShipClassifier.shipCount];
    public ShipClassifier selectedShip;

    private TargetingSystem targetingSystemScript;

    private readonly GameObject[] archers = new GameObject[ArrowShoot.totalArcherCount];
    private readonly GameObject[] gunmen = new GameObject[GunShoot.totalGunmanCount];
    private readonly GameObject[] shootUnitCannon = new GameObject[CannonController.totalCannonCount];
    private readonly GameObject[] shootUnitMortar = new GameObject[MortarController.totalMortarCount];

    private readonly ArcherController[] archerControllerScript = new ArcherController[ArrowShoot.totalArcherCount];
    private readonly GunmanController[] gunmanControllerScript = new GunmanController[GunShoot.totalGunmanCount];
    private readonly CannonController[] cannonControllerScript = new CannonController[CannonController.totalCannonCount];
    private readonly MortarController[] mortarControllerScript = new MortarController[MortarController.totalMortarCount];

    private ShipClassifier shipClassifierScript;

    private ArrowShoot arrowShootScript;
    private GunShoot gunShootScript;

    private bool isNavyShip;
    private bool isActiveShip;

    private float maxRange;
    private float mortarRange;
    private int selectedIndex = 0;
    private int shipsInRangeCount = 0;

    private string shooter;
    private string shooter2;

    public GameObject activeMarker;

    private GameObject scaleFactorGameObject;
    private GameObject parentShooterObject;
    private GameObject parentShooterObject2;

    private GameObject myShipCenter;
    private Vector3 myShipPosition;

    private bool selectedShipNotInRange = true;

    private void Awake()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject gameObject = this.transform.GetChild(i).gameObject;
            if (gameObject.name == "ScaleFactorGameObject")
            {
                scaleFactorGameObject = gameObject;
            }
            else if (gameObject.name == "activeMarker")
            {
                activeMarker = gameObject;
            }
        }
        for (int i = 0; i < scaleFactorGameObject.transform.childCount; i++)
        {
            GameObject gameObject = scaleFactorGameObject.transform.GetChild(i).gameObject;
            if (gameObject.name == "Archers" || gameObject.name == "Gunmen")
            {
                parentShooterObject = gameObject;
                shooter = parentShooterObject.name;
                parentShooterObject2 = null;
                shooter2 = null;
            }
            else if (gameObject.name == "CannonUnit")
            {
                parentShooterObject = gameObject;
                shooter = parentShooterObject.name;
            }
            else if (gameObject.name == "MortarUnit")
            {
                parentShooterObject2 = gameObject;
                shooter2 = parentShooterObject2.name;
            }
        }
        for (int i = 0; i < parentShooterObject.transform.childCount; i++)
        {
            if (shooter == "Archers")
            {
                archers[i] = parentShooterObject.transform.GetChild(i).gameObject;
                archerControllerScript[i] = archers[i].GetComponent<ArcherController>();
            }
            else if (shooter == "Gunmen")
            {
                gunmen[i] = parentShooterObject.transform.GetChild(i).gameObject;
                gunmanControllerScript[i] = gunmen[i].GetComponent<GunmanController>();
            }
            else if (shooter == "CannonUnit")
            {
                shootUnitCannon[i] = parentShooterObject.transform.GetChild(i).gameObject;
                cannonControllerScript[i] = shootUnitCannon[i].GetComponent<CannonController>();
            }
        }
        if (parentShooterObject2 != null)
        {
            for (int i = 0; i < parentShooterObject2.transform.childCount; i++)
            {
                shootUnitMortar[i] = parentShooterObject2.transform.GetChild(i).gameObject;
                mortarControllerScript[i] = shootUnitMortar[i].GetComponent<MortarController>();
            }
        }
        shipClassifierScript = this.GetComponent<ShipClassifier>();
    }

    private void Start()
    {
        activeMarker.SetActive(false);
        if (shooter == "Archers")
        {
            arrowShootScript = this.GetComponent<ArrowShoot>();
            maxRange = arrowShootScript.archerMaxRange;
            gunShootScript = null;
        }
        else if (shooter == "Gunmen")
        {
            gunShootScript = this.GetComponent<GunShoot>();
            maxRange = gunShootScript.gunmanMaxRange;
            arrowShootScript = null;
        }
        else if (shooter == "CannonUnit")
        {
            maxRange = cannonControllerScript[0].cannonMaxRange;
            arrowShootScript = null;
            gunShootScript = null;
        }

        if (shooter2 == "MortarUnit")
        {
            mortarRange = mortarControllerScript[0].mortarMaxRange;
        }

        //we won't set maxrange to that of mortarMaxRange as both cannon and mortar are on same ship, and cannon has larger range than mortar

        isNavyShip = shipClassifierScript.isNavyShip;
        for (int i = 0; i < ShipClassifier.shipCount; i++)
        {
            shipsInRange[i] = null;
        }
        myShipCenter = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        myShipPosition = myShipCenter.transform.position;
        isActiveShip = shipClassifierScript.isActive;

        if (shooter == "Archers" || shooter == "Gunmen")
        {
            if (isNavyShip)
            {
                CheckShipList1(ShipClassifier.GetPirateShipList());
            }
            else
            {
                CheckShipList1(ShipClassifier.GetNavyShipList());
            }
        }

        if (shooter == "CannonUnit" && shooter2 == "MortarUnit")
        {
            if (isNavyShip)
            {
                CheckShipList2(ShipClassifier.GetPirateShipList());
            }
            else//isPirateShip
            {
                CheckShipList2(ShipClassifier.GetNavyShipList());
            }
        }

        if (shipsInRangeCount == 0)
        {
            if (shooter == "Archers")
            {
                for (int i = 0; i < ArrowShoot.totalArcherCount; i++)
                {
                    archerControllerScript[i].B = null;
                }
            }
            else if (shooter == "Gunmen")
            {
                for (int i = 0; i < GunShoot.totalGunmanCount; i++)
                {
                    gunmanControllerScript[i].B = null;
                }
            }
            else if (shooter == "CannonUnit")
            {
                for (int i = 0; i < CannonController.totalCannonCount; i++)
                {
                    cannonControllerScript[i].B = null;
                }
            }

            if (shooter2 == "MortarUnit")
            {
                for (int i = 0; i < MortarController.totalMortarCount; i++)
                {
                    mortarControllerScript[i].B = null;
                }
            }
            selectedShip = null;                    
        }
        else if (shipsInRangeCount == 1)
        {
            foreach (ShipClassifier enemyShip in shipsInRange)
            {
                if (enemyShip != null)
                {
                    selectedShip = enemyShip;

                    if (shooter == "Archers")
                    {
                        for (int i = 0; i < ArrowShoot.totalArcherCount; i++)
                        {
                            GameObject enemyShipCenter = enemyShip.transform.GetChild(0).gameObject;
                            archerControllerScript[i].B = enemyShipCenter.transform;
                        }
                    }
                    else if (shooter == "Gunmen")
                    {
                        for (int i = 0; i < GunShoot.totalGunmanCount; i++)
                        {
                            GameObject enemyShipCenter = enemyShip.transform.GetChild(0).gameObject;
                            gunmanControllerScript[i].B = enemyShipCenter.transform;
                        }
                    }
                    else if (shooter == "CannonUnit")
                    {
                        for (int i = 0; i < CannonController.totalCannonCount; i++)
                        {
                            GameObject enemyShipCenter = enemyShip.transform.GetChild(0).gameObject;
                            cannonControllerScript[i].B = enemyShipCenter.transform;
                        }
                    }

                    if (shooter2 == "MortarUnit")
                    {
                        for (int i = 0; i < MortarController.totalMortarCount; i++)
                        {
                            GameObject enemyShipCenter = enemyShip.transform.GetChild(0).gameObject;
                            mortarControllerScript[i].B = enemyShipCenter.transform;
                        }
                    }
                }
            }
        }      
        else if (shipsInRangeCount > 1)//Make sure that the new array stores the ships along a circular direction
        {
            //Check whether selectedShip is the ship in range(we encountered case when a ship not in range was selected ship)
            if (selectedShip != null)
            {
                foreach (ShipClassifier enemyShip in shipsInRange)
                {
                    if (selectedShip == enemyShip)
                    {
                        selectedShipNotInRange = false;
                    }
                }
                if (selectedShipNotInRange)
                {
                    selectedShip = null;
                }
                selectedShipNotInRange = true;
            }
            else
            {
                foreach (ShipClassifier enemyShip in shipsInRange)
                {
                    if (enemyShip != null)
                    {
                        selectedShip = enemyShip;
                        break;
                    }
                }
            }

            if (selectedShip != null)
            {
                //1. Copy all elements from shipsInRange to circularlyArrangedShips
                for (int i = 0; i < ShipClassifier.shipCount; i++)
                {
                    circularlyArrangedShips[i] = shipsInRange[i];
                }

                //2. Make sure that null values lie at end of array, non-null elements are all at front of array
                System.Array.Sort(circularlyArrangedShips, (x, y) =>
                {
                    if (x == null && y == null)
                        return 0;//randomly determine to replace or not
                    if (x == null)
                        return 1;//replace 2nd element with 1st
                    if (y == null)
                        return -1;//no need to replace 2nd element with 1st
                                  // For non-null elements, you can use any comparison logic or leave it arbitrary
                    return -1;
                });

                //3. Sort those elements according to angle, so they are now circularly arranged in new array
                ShipSort.SortShips(circularlyArrangedShips, gameObject);

                //start from the element of array that was being targeted at when count was equal to 1, continue making B.position equal to this ship
                //B.pos = selected ship inside new array, so search at which pos selected ship is and put selectedIndex equal to that value
                for (int i = 0; i < shipsInRangeCount; i++)
                {
                    if (circularlyArrangedShips[i] == selectedShip)
                    {
                        selectedIndex = i;
                    }
                }

                if (shooter == "Archers")
                {
                    for (int i = 0; i < ArrowShoot.totalArcherCount; i++)
                    {
                        GameObject selectedShipCenter = selectedShip.transform.GetChild(0).gameObject;
                        archerControllerScript[i].B = selectedShipCenter.transform;
                    }
                }
                else if (shooter == "Gunmen")
                {
                    for (int i = 0; i < GunShoot.totalGunmanCount; i++)
                    {
                        GameObject selectedShipCenter = selectedShip.transform.GetChild(0).gameObject;
                        gunmanControllerScript[i].B = selectedShipCenter.transform;
                    }
                }
                else if (shooter == "CannonUnit")
                {
                    for (int i = 0; i < CannonController.totalCannonCount; i++)
                    {
                        GameObject selectedShipCenter = selectedShip.transform.GetChild(0).gameObject;
                        cannonControllerScript[i].B = selectedShipCenter.transform;
                    }
                }

                if (shooter2 == "MortarUnit")
                {
                    for (int i = 0; i < MortarController.totalMortarCount; i++)
                    {
                        GameObject selectedShipCenter = selectedShip.transform.GetChild(0).gameObject;
                        mortarControllerScript[i].B = selectedShipCenter.transform;
                    }
                }

                //now when R is pressed, select the ship at next index from selectedIndex and so on
                //when L is pressed, select ship at index left and so on

                if (Input.GetKeyDown(KeyCode.R) && isActiveShip)
                {
                    SelectNextShipClockwise();
                }
                else if (Input.GetKeyDown(KeyCode.L) && isActiveShip)
                {
                    SelectNextShipCounterclockwise();
                }          
            }
        }

        //Assign marker to ships
        if (shipsInRangeCount >= 1)
        {
            if (isNavyShip)
            {
                foreach (ShipClassifier pirateEnemyShip in ShipClassifier.GetPirateShipList())
                {
                    if (isActiveShip)
                    {
                        if (pirateEnemyShip != selectedShip)
                        {
                            targetingSystemScript = pirateEnemyShip.GetComponent<TargetingSystem>();
                            if (targetingSystemScript.activeMarker.activeSelf)
                            {
                                targetingSystemScript.activeMarker.SetActive(false);
                            }
                        }
                        else
                        {
                            targetingSystemScript = pirateEnemyShip.GetComponent<TargetingSystem>();
                            if (!targetingSystemScript.activeMarker.activeSelf)
                            {
                                targetingSystemScript.activeMarker.SetActive(true);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (ShipClassifier navyEnemyShip in ShipClassifier.GetNavyShipList())
                {
                    if (isActiveShip)
                    {
                        if (navyEnemyShip != selectedShip)
                        {
                            targetingSystemScript = navyEnemyShip.GetComponent<TargetingSystem>();
                            if (targetingSystemScript.activeMarker.activeSelf)
                            {
                                targetingSystemScript.activeMarker.SetActive(false);
                            }
                        }
                        else
                        {
                            targetingSystemScript = navyEnemyShip.GetComponent<TargetingSystem>();
                            if (!targetingSystemScript.activeMarker.activeSelf)
                            {
                                targetingSystemScript.activeMarker.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
        else//shipsInRangeCount == 0
        {
            if (isActiveShip)
            {
                if (isNavyShip)
                {
                    foreach (ShipClassifier pirateEnemyShip in ShipClassifier.GetPirateShipList())
                    {
                        targetingSystemScript = pirateEnemyShip.GetComponent<TargetingSystem>();
                        if (targetingSystemScript.activeMarker.activeSelf)
                        {
                            targetingSystemScript.activeMarker.SetActive(false);
                        }
                    }
                }
                else
                {
                    foreach (ShipClassifier navyEnemyShip in ShipClassifier.GetNavyShipList())
                    {
                        targetingSystemScript = navyEnemyShip.GetComponent<TargetingSystem>();
                        if (targetingSystemScript.activeMarker.activeSelf)
                        {
                            targetingSystemScript.activeMarker.SetActive(false);
                        }
                    }
                }
            }         
        }

        //Debug multiple markers case, if any pirate/navy ship is active, then any other pirate/navy ship cannot have its own marker active
        if (isNavyShip && isActiveShip)
        {
            foreach (ShipClassifier navyEnemyShip in ShipClassifier.GetNavyShipList())
            {
                targetingSystemScript = navyEnemyShip.GetComponent<TargetingSystem>();
                if (targetingSystemScript.activeMarker.activeSelf)
                {
                    targetingSystemScript.activeMarker.SetActive(false);
                }
            }
        }
        else if (!isNavyShip && isActiveShip)
        {
            foreach (ShipClassifier pirateEnemyShip in ShipClassifier.GetPirateShipList())
            {
                targetingSystemScript = pirateEnemyShip.GetComponent<TargetingSystem>();
                if (targetingSystemScript.activeMarker.activeSelf)
                {
                    targetingSystemScript.activeMarker.SetActive(false);
                }
            }
        }
    }
    private void SelectNextShipClockwise()
    {
        SelectNextShip('R');
    }
    private void SelectNextShipCounterclockwise()
    {
        SelectNextShip('L');
    }
    private void CheckShipList1(IEnumerable<ShipClassifier> enemyShips)
    {
        foreach (ShipClassifier enemyShip in enemyShips)
        {
            bool foundInInnerLoop = false;
            GameObject enemyShipCenter = enemyShip.transform.GetChild(0).gameObject;
            float distanceToEnemyShipCenter = Vector3.Distance(myShipPosition, enemyShipCenter.transform.position);

            if (distanceToEnemyShipCenter < maxRange)
            {
                for (int i = 0; i < ShipClassifier.shipCount; i++)
                {
                    if (shipsInRange[i] == enemyShip)
                    {
                        foundInInnerLoop = true;
                        break;
                    }
                }
                if (foundInInnerLoop)
                {
                    continue;
                }

                AddToShipsInRange(enemyShip);
            }
            else
            {
                RemoveFromShipsInRange(enemyShip);
            }
        }
    }
    private void CheckShipList2(IEnumerable<ShipClassifier> enemyShips)
    {
        foreach (ShipClassifier enemyShip in enemyShips)
        {
            bool foundInInnerLoop = false;
            GameObject enemyShipCenter = enemyShip.transform.GetChild(0).gameObject;
            float distanceToEnemyShipCenter = Vector3.Distance(myShipPosition, enemyShipCenter.transform.position);

            if (distanceToEnemyShipCenter < maxRange)
            {
                //Mortar Check
                bool addToShipsInRangeArray = IsWithinMortarRange(distanceToEnemyShipCenter);

                //Cannon Range Check
                if (!addToShipsInRangeArray)
                {
                    addToShipsInRangeArray = IsWithinCannonRange(enemyShipCenter);
                }

                if (addToShipsInRangeArray)
                {
                    for (int i = 0; i < ShipClassifier.shipCount; i++)
                    {
                        if (shipsInRange[i] == enemyShip)
                        {
                            foundInInnerLoop = true;
                            break;
                        }
                    }
                    if (foundInInnerLoop)
                    {
                        continue;
                    }

                    AddToShipsInRange(enemyShip);
                }

                //remove from list if doesnot fulfill mortar distance or cannon range criteria
                if (!addToShipsInRangeArray)
                {
                    RemoveFromShipsInRange(enemyShip);
                }
            }
            else
            {
                RemoveFromShipsInRange(enemyShip);
            }
        }
    }
    private bool IsWithinMortarRange(float distanceToEnemyCenter)
    {
        return distanceToEnemyCenter < mortarRange;
    }
    private bool IsWithinCannonRange(GameObject enemyShipCenter)
    {
        for (int i = 0; i < CannonController.totalCannonCount; i++)
        {
            GameObject newCannon = cannonControllerScript[i].newCannon;
            Transform B = enemyShipCenter.transform;
            float cannonShootAngleRange = cannonControllerScript[i].cannonShootAngleRange;

            if (cannonControllerScript[i].CannonRangeCheck(newCannon, B, cannonShootAngleRange))
            {
                return true;
            }
        }

        return false;
    }

    private void IsShipAlreadyInList(ShipClassifier enemyShip, bool foundInInnerLoop)
    {

    }

    private void AddToShipsInRange(ShipClassifier enemyShip)
    {
        for (int i = 0; i < ShipClassifier.shipCount; i++)
        {
            if (shipsInRange[i] == null)
            {
                shipsInRange[i] = enemyShip;
                shipsInRangeCount++;
                return;
            }
        }
    }
    private void RemoveFromShipsInRange(ShipClassifier enemyShip)
    {
        for (int i = 0; i < ShipClassifier.shipCount; i++)
        {
            if (shipsInRange[i] == enemyShip)
            {
                shipsInRange[i] = null;
                circularlyArrangedShips[i] = null;
                shipsInRangeCount--;
                return;
            }
        }
    }

    private int CalculateNextShipIndex(char direction, int currentShipIndex)
    {
        int nextShipIndex;
        if (direction == 'R')
        {
            nextShipIndex = (currentShipIndex + 1) % shipsInRangeCount;
        }
        else//direction == 'L'
        {
            nextShipIndex = (currentShipIndex - 1 + shipsInRangeCount) % shipsInRangeCount;
        }
        return nextShipIndex;
    }
    private void SelectNextShip(char direction)
    {
        SetActiveMarker(false);
        selectedIndex = CalculateNextShipIndex(direction, selectedIndex);
        selectedShip = circularlyArrangedShips[selectedIndex];
        SetActiveMarker(true);
    }
    private void SetActiveMarker(bool temp)
    {
        targetingSystemScript = selectedShip.GetComponent<TargetingSystem>();
        targetingSystemScript.activeMarker.SetActive(temp);
    }
}