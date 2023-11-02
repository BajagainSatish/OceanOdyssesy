using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ShipSort
{
    public static void SortShips(ShipClassifier[] neighboringShips, GameObject mainShip)
    {
        // Remove null elements from the array
        ShipClassifier[] tempNeighboringShips = neighboringShips.Where(ship => ship != null).ToArray();

        // Get the position of the main ship
        Vector3 mainShipPos = mainShip.transform.position;

        // Sort by angle relative to the main ship
        Array.Sort(tempNeighboringShips, (a, b) =>
        {
            Vector3 dirA = a.transform.position - mainShipPos;
            Vector3 dirB = b.transform.position - mainShipPos;
            float angleA = Mathf.Atan2(dirA.x, dirA.z);
            float angleB = Mathf.Atan2(dirB.x, dirB.z);

            if (angleA < angleB)
            {
                return -1;
            }
            if (angleA > angleB)
            {
                return 1;
            }

            // If angles are the same, sort by distance
            float distanceA = Vector3.Distance(a.transform.position, mainShipPos);
            float distanceB = Vector3.Distance(b.transform.position, mainShipPos);

            if (distanceA < distanceB)
            {
                return -1;
            }
            if (distanceA > distanceB)
            {
                return 1;
            }

            // If angles and distances are the same, sort by unique identifier (e.g., ship name)
            return a.name.CompareTo(b.name);
        });

        int newLength = tempNeighboringShips.Length;

        if (newLength < neighboringShips.Length)
        {
            for (int i = 0; i < newLength; i++)
            {
                neighboringShips[i] = tempNeighboringShips[i];
            }
            for (int i = newLength; i < neighboringShips.Length; i++)
            {
                neighboringShips[i] = null;
            }
        }
        else if (newLength == neighboringShips.Length)
        {
            for (int i = 0; i < neighboringShips.Length; i++)
            {
                neighboringShips[i] = tempNeighboringShips[i];
            }
        }
    }
}
