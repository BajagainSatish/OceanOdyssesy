using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipClassifier : MonoBehaviour
{
    public static int shipCount = 4;
    private static ShipClassifier[] pirateShipList = new ShipClassifier[shipCount];
    private static ShipClassifier[] navyShipList = new ShipClassifier[shipCount];

    public bool isNavyShip;
    public static ShipClassifier[] GetPirateShipList()
    {
        return pirateShipList;
    }

    public static ShipClassifier[] GetNavyShipList()
    {
        return navyShipList;
    }
    private void Awake()
    {
        if (isNavyShip)
        {
            for (int i = 0; i < shipCount; i++)
            {
                if (navyShipList[i] == null)
                {
                    navyShipList[i] = this;
                    return;
                }
            }
        }
        else
        {
            for (int i = 0; i < shipCount; i++)
            {
                if (pirateShipList[i] == null)
                {
                    pirateShipList[i] = this;
                    return;
                }
            }
        }
    }
}
