using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipClassifier : MonoBehaviour
{
    public static int shipCount = 7;
    private static ShipClassifier[] pirateShipList = new ShipClassifier[shipCount];
    private static ShipClassifier[] navyShipList = new ShipClassifier[shipCount];

    public bool isNavyShip;
    public bool isActive;

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
            AddNavyShipToNavyShipList();
        }
        else
        {
            AddPirateShipToPirateShipList();
        }
    }

    private void Start()
    {
        //Check if each ship's 0th child is ShipCenter
        if (transform.GetChild(0).name != "ShipCenter")
        {
            Debug.LogWarning("Ensure that 0th child is ShipCenter for proper distance calculation between ships!!!" + this.name);
        }
    }

    private void AddNavyShipToNavyShipList()
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

    private void AddPirateShipToPirateShipList()
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
