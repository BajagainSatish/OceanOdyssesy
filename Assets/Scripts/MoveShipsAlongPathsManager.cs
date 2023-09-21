using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveShipsAlongPathsManager : MonoBehaviour
{
    public MoveObjectAlongSpline[] splinePathMover;
    private GameObject[] shipsToSelectFrom; //the full collection of ships to select from
    private GameObject[] selectedships; //the actual objects selected from full collection to move in path
    private int noOfShipsToSelect;  //the number of ships to select randomly to move in random path //should be less than or equal to the number of paths

    public void SetShipsToSelectFrom(GameObject[] shipObjects, int n)
    {
        shipsToSelectFrom = (GameObject[])shipObjects.Clone();  //create new shallow copy so as to not change original array which might be needed by others;
        noOfShipsToSelect = Mathf.Clamp(n, 0, shipsToSelectFrom.Length);
        noOfShipsToSelect = Mathf.Clamp(noOfShipsToSelect, 0, splinePathMover.Length);  //limit the selectionNumber by avialable path and ships
    }

    public GameObject[] GetSelectedShips()
    {
        return selectedships;
    }

    public void Initialize()
    {
        ResetPathObjects();
        RandomizeShips();
        RandomizePaths();
        SetSelectedShips();
    }


    private void SetSelectedShips()
    {//selects ships from the full collection of ships and assigns to random paths
        selectedships = new GameObject[noOfShipsToSelect];
        foreach(MoveObjectAlongSpline path in splinePathMover)
            path.gameObject.SetActive(false); //hide all path elements to not show non-selected paths
        foreach(GameObject ship in shipsToSelectFrom)
            ship.SetActive(false);

        for (int i = 0; i < selectedships.Length; i++)
        {
            //having the selected ships equate to first consecutive elememts simplifies when there is need to find corresponding splinepathmover and so on
            selectedships[i] = shipsToSelectFrom[i];
            splinePathMover[i].SetObjectToMove(selectedships[i]);

            //show only selected paths and ships
            splinePathMover[i].gameObject.SetActive(true);
            shipsToSelectFrom[i].SetActive(true);
        }
    }

    private void RandomizeShips()
    {//shuffles the ship array
        for(int i = shipsToSelectFrom.Length - 1; i > 0; i --)
        {
            int swapPos = Random.Range(0, i+1); //swap each elememt randomly with remaining
            GameObject temp = shipsToSelectFrom[swapPos];
            shipsToSelectFrom[swapPos] = shipsToSelectFrom[i];
            shipsToSelectFrom[i] = temp;
        }
    }

    private void ResetPathObjects()
    {
        foreach(MoveObjectAlongSpline splineMover in splinePathMover)
            splineMover.RemoveExistingMovingObject();
    }
    private void RandomizePaths()
    {//shuffles spline paths so that ships are randomly placed on available paths
        splinePathMover = (MoveObjectAlongSpline[])splinePathMover.Clone();  //create new shallow copy so as to not change original array which might be needed by others
        for (int i = splinePathMover.Length - 1; i > 0; i--)
        {
            int swapPos = Random.Range(0, i + 1);  //swap each elememt randomly with remaining
            MoveObjectAlongSpline temp = splinePathMover[swapPos];
            splinePathMover[swapPos] = splinePathMover[i];
            splinePathMover[i] = temp;
        }
    }

    public void SetMovementSpeed(int indexOfShip, float speed)
    {
        splinePathMover[indexOfShip].SetSpeed(speed);
    }


    public void MoveShip(int i)
    {//start movement for ith ship
        if (selectedships == null)
            return;
        splinePathMover[i].Move();
    }
    public void MoveAllShips()
    {
        if (selectedships == null)
            return;
        for(int i=0; i< selectedships.Length; i++)
            splinePathMover[i].Move();
    }

    public void StopShip(int i)
    {//pause movement for ith ship
        if (selectedships == null)
            return;
        splinePathMover[i].Stop();
    }
    public void StopAllShips()
    {
        if (selectedships == null)
            return;
        for (int i = 0; i < selectedships.Length; i++)
            splinePathMover[i].Stop();
    }

    public void ResetShip(int i, bool autoplay = false)
    {
        if (selectedships == null)
            return;
        splinePathMover[i].ResetObject(autoplay);
    }
    public void ResetAllShipPositions(bool autoplay = false)
    {
        if (selectedships == null)
            return;
        for (int i = 0; i < selectedships.Length; i++)
            splinePathMover[i].ResetObject(autoplay);
    }

}
