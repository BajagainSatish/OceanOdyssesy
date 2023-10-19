using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    [SerializeField] private float maxRange;
    private ShipClassifier[] shipsInRange = new ShipClassifier[ShipClassifier.shipCount];
    private ShipClassifier[] circularlyArrangedShips = new ShipClassifier[ShipClassifier.shipCount];
    private ShipClassifier selectedShip;

    private GameObject[] archers = new GameObject[ArrowShoot.totalArcherCount];
    private ArcherController[] archerControllerScript = new ArcherController[ArrowShoot.totalArcherCount];

    private int selectedIndex = 0;
    private bool isNavyShip;
    private int shipsInRangeCount = 0;

    private GameObject scaleFactorGameObject;
    private GameObject pirates;

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
            if (gameObject.name == "Pirates")
            {
                pirates = gameObject;
            }
        }
        for (int i = 0; i < pirates.transform.childCount; i++)
        {
            archers[i] = pirates.transform.GetChild(i).gameObject;
            archerControllerScript[i] = archers[i].GetComponent<ArcherController>();
        }
    }

    private void Start()
    {
        isNavyShip = this.GetComponent<ShipClassifier>().isNavyShip;
        for (int i = 0; i < ShipClassifier.shipCount; i++)
        {
            shipsInRange[i] = null;
        }
    }

    private void Update()
    {
        /*
            To display arrow over the currently selected ship, just implement that to selectedShip inside Update method.
            eg. ArrowDisplay(selectedShip);

            To switch left or right, just replace keycode.L and keycode.R by buttons
         */
        if (isNavyShip)
        {
            foreach (ShipClassifier pirateEnemyShip in ShipClassifier.GetPirateShipList())
            {
                bool foundInInnerLoop = false;
                if (Vector3.Distance(transform.position, pirateEnemyShip.transform.position) < maxRange)
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
                if (Vector3.Distance(transform.position, navyEnemyShip.transform.position) < maxRange)
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

        if (shipsInRangeCount == 1)
        {
            foreach (ShipClassifier enemyShip in shipsInRange)
            {
                if (enemyShip != null)
                {
                    selectedShip = enemyShip;
                    //print("Selected ship: " + selectedShip);
                    for (int i = 0; i < ArrowShoot.totalArcherCount; i++)
                    {
                        GameObject pirateShipCenter = enemyShip.transform.GetChild(0).gameObject;
                        archerControllerScript[i].B = pirateShipCenter.transform;
                    }
                }
            }
        }      
        else if (shipsInRangeCount > 1)//Make sure that the new array stores the ships along a circular direction
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

            //3. Sort those elements
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
            for (int i = 0; i < ArrowShoot.totalArcherCount; i++)
            {
                GameObject pirateShipCenter = selectedShip.transform.GetChild(0).gameObject;
                archerControllerScript[i].B = pirateShipCenter.transform;
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
    private void SelectNextShipClockwise()
    {
        selectedIndex = (selectedIndex + 1) % shipsInRangeCount;
        selectedShip = circularlyArrangedShips[selectedIndex];
        print("Selected Ship " + selectedShip);
    }

    private void SelectNextShipCounterclockwise()
    {
        selectedIndex = (selectedIndex - 1 + shipsInRangeCount) % shipsInRangeCount;
        selectedShip = circularlyArrangedShips[selectedIndex];
        print("Selected Ship " + selectedShip);
    }
}