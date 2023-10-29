using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    private ShipClassifier[] shipsInRange = new ShipClassifier[ShipClassifier.shipCount];
    private ShipClassifier[] circularlyArrangedShips = new ShipClassifier[ShipClassifier.shipCount];
    private ShipClassifier selectedShip;

    private GameObject[] archers = new GameObject[ArrowShoot.totalArcherCount];
    private GameObject[] gunmen = new GameObject[GunShoot.totalGunmanCount];
    private GameObject[] shootUnitCannon = new GameObject[CanonController.totalCannonCount];

    private ArcherController[] archerControllerScript = new ArcherController[ArrowShoot.totalArcherCount];
    private GunmanController[] gunmanControllerScript = new GunmanController[GunShoot.totalGunmanCount];
    private CanonController[] canonControllerScript = new CanonController[CanonController.totalCannonCount];

    private ArrowShoot arrowShootScript;
    private GunShoot gunShootScript;

    private bool isNavyShip;
    private float maxRange;
    private int selectedIndex = 0;
    private int shipsInRangeCount = 0;
    private string shooter;

    private GameObject scaleFactorGameObject;
    private GameObject parentShooterObject;
    private GameObject myShipCenter;
    private Vector3 myShipPosition;

    [SerializeField] private bool displayMyContents = false;
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
        }
        for (int i = 0; i < scaleFactorGameObject.transform.childCount; i++)
        {
            GameObject gameObject = scaleFactorGameObject.transform.GetChild(i).gameObject;
            if (gameObject.name == "Archers" || gameObject.name == "Gunmen" || gameObject.name == "CannonUnit")
            {
                parentShooterObject = gameObject;
                shooter = parentShooterObject.name;
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
                canonControllerScript[i] = shootUnitCannon[i].GetComponent<CanonController>();
            }
        }
    }

    private void Start()
    {
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
            maxRange = canonControllerScript[0].cannonMaxRange;
            arrowShootScript = null;
            gunShootScript = null;
        }

        isNavyShip = this.GetComponent<ShipClassifier>().isNavyShip;
        for (int i = 0; i < ShipClassifier.shipCount; i++)
        {
            shipsInRange[i] = null;
        }
        myShipCenter = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        /*
           To display arrow over the currently selected ship, just implement that to selectedShip inside Update method.
           eg. ArrowDisplay(selectedShip);

           To switch left or right, just replace keycode.L and keycode.R by buttons
        */

        if (displayMyContents)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                print("shipcount: " + shipsInRangeCount);
                if (selectedShip != null)
                {
                    print("Selected Ship: " + selectedShip.name);
                }
                else
                {
                    print("Selected Ship: " + " null");
                }

                print("All ships in range");
                for (int i = 0; i < shipsInRangeCount; i++)
                {
                    print(shipsInRange[i].name);
                }
            }
        }

        myShipPosition = myShipCenter.transform.position;
        if (isNavyShip)
        {
            foreach (ShipClassifier pirateEnemyShip in ShipClassifier.GetPirateShipList())
            {
                bool foundInInnerLoop = false;
                GameObject pirateEnemyShipCenter = pirateEnemyShip.transform.GetChild(0).gameObject;

                if (Vector3.Distance(myShipPosition, pirateEnemyShipCenter.transform.position) < maxRange)
                {
                    for (int i = 0; i < ShipClassifier.shipCount; i++)
                    {
                        if (shipsInRange[i] == pirateEnemyShip)
                        {
                            foundInInnerLoop = true;
                            break;
                        }
                    }
                    if (foundInInnerLoop)
                    {
                        continue;
                    }
                    for (int i = 0; i < ShipClassifier.shipCount; i++)
                    {
                        if (shipsInRange[i] == null)
                        {
                            shipsInRange[i] = pirateEnemyShip;
                            shipsInRangeCount++;
                            return;
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < ShipClassifier.shipCount; i++)
                    {
                        if (shipsInRange[i] == pirateEnemyShip)
                        {
                            shipsInRange[i] = null;
                            circularlyArrangedShips[i] = null;
                            shipsInRangeCount--;
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            foreach (ShipClassifier navyEnemyShip in ShipClassifier.GetNavyShipList())
            {
                bool foundInInnerLoop = false;
                GameObject navyEnemyShipCenter = navyEnemyShip.transform.GetChild(0).gameObject;
                if (Vector3.Distance(myShipPosition, navyEnemyShipCenter.transform.position) < maxRange)
                {
                    for (int i = 0; i < ShipClassifier.shipCount; i++)
                    {
                        if (shipsInRange[i] == navyEnemyShip)
                        {
                            foundInInnerLoop = true;
                            break;
                        }
                    }
                    if (foundInInnerLoop)
                    {
                        continue;
                    }
                    for (int i = 0; i < ShipClassifier.shipCount; i++)
                    {
                        if (shipsInRange[i] == null)
                        {
                            shipsInRange[i] = navyEnemyShip;
                            shipsInRangeCount++;
                            return;
                        }
                    }

                }
                else
                {
                    for (int i = 0; i < ShipClassifier.shipCount; i++)
                    {
                        if (shipsInRange[i] == navyEnemyShip)
                        {
                            shipsInRange[i] = null;
                            circularlyArrangedShips[i] = null;
                            shipsInRangeCount--;
                            return;
                        }
                    }
                }
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
                for (int i = 0; i < CanonController.totalCannonCount; i++)
                {
                    canonControllerScript[i].B = null;
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
                        for (int i = 0; i < CanonController.totalCannonCount; i++)
                        {
                            GameObject enemyShipCenter = enemyShip.transform.GetChild(0).gameObject;
                            canonControllerScript[i].B = enemyShipCenter.transform;
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
                ShipSort.SortShips(circularlyArrangedShips, this.gameObject);

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
                    for (int i = 0; i < CanonController.totalCannonCount; i++)
                    {
                        GameObject selectedShipCenter = selectedShip.transform.GetChild(0).gameObject;
                        canonControllerScript[i].B = selectedShipCenter.transform;
                    }
                }

                //now when R is pressed, select the ship at next index from selectedIndex and so on
                //when L is pressed, select ship at index left and so on

                if (Input.GetKeyDown(KeyCode.R))
                {
                    SelectNextShipClockwise();
                }
                else if (Input.GetKeyDown(KeyCode.L))
                {
                    SelectNextShipCounterclockwise();
                }
            }            
        }      
    }
    private void SelectNextShipClockwise()
    {
        selectedIndex = (selectedIndex + 1) % shipsInRangeCount;
        selectedShip = circularlyArrangedShips[selectedIndex];
        //print("Selected Ship " + selectedShip);
    }

    private void SelectNextShipCounterclockwise()
    {
        selectedIndex = (selectedIndex - 1 + shipsInRangeCount) % shipsInRangeCount;
        selectedShip = circularlyArrangedShips[selectedIndex];
        //print("Selected Ship " + selectedShip);
    }
}