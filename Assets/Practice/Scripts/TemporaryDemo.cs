using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TemporaryDemo : MonoBehaviour
{
    [SerializeField] private static int totalShipInSceneCount = 14;
    [SerializeField] private GameObject[] allShipsInScene = new GameObject[totalShipInSceneCount];

    private readonly ShipClassifier[] shipClassifierScript = new ShipClassifier[totalShipInSceneCount];
    private ShipClassifier currentActiveShip;

    private TargetingSystem targetingSystemScript;

    private int activeShipIndex;

    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;
    [SerializeField] private float zOffset;

    [SerializeField] private float defaultXCameraPos;
    [SerializeField] private float defaultYCameraPos;
    [SerializeField] private float defaultZCameraPos;

    [SerializeField] private float defaultXCameraRot;
    [SerializeField] private float defaultYCameraRot;
    [SerializeField] private float defaultZCameraRot;

    private void Awake()
    {
        for (int i = 0; i < totalShipInSceneCount; i++)
        {
            shipClassifierScript[i] = allShipsInScene[i].GetComponent<ShipClassifier>();
        }
    }

    private void Start()
    {
        for (int i = 0; i < totalShipInSceneCount; i++)
        {
            shipClassifierScript[i].isActive = false;
        }
        activeShipIndex = 0;
    }

    private void Update()
    {
        //Press on space reset control from all ships, or gain control to any one of the ships
        //Press on left and right arrow keys allow to switch between ships
        //Press on R and L keys allow to switch the aiming of current ship towards multiple enemy ships in range
        //Press on S shoots the projectile for all ships that have found a target

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentActiveShip == null)
            {
                GainShipControl();
            }
            else if (currentActiveShip != null)
            {
                LoseShipControl();
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectNextShip('R');
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectNextShip('L');
        }

        //Temporary Camera Control
        EvaluateCameraPosition();

        //Arrow marker control, when no ship is selected, deactivate all markers
        if (currentActiveShip == null)
        {
            DeactivateAllMarkers();
        }
    }

    private void DeactivateAllMarkers()
    {
        foreach (ShipClassifier navyEnemyShip in ShipClassifier.GetNavyShipList())
        {
            targetingSystemScript = navyEnemyShip.GetComponent<TargetingSystem>();
            if (targetingSystemScript.activeMarker.activeSelf)
            {
                targetingSystemScript.activeMarker.SetActive(false);
            }
        }
        foreach (ShipClassifier pirateEnemyShip in ShipClassifier.GetPirateShipList())
        {
            targetingSystemScript = pirateEnemyShip.GetComponent<TargetingSystem>();
            if (targetingSystemScript.activeMarker.activeSelf)
            {
                targetingSystemScript.activeMarker.SetActive(false);
            }
        }
    }

    private void GainShipControl()
    {
        currentActiveShip = allShipsInScene[activeShipIndex].GetComponent<ShipClassifier>();
        currentActiveShip.isActive = true;
    }

    private void LoseShipControl()
    {
        currentActiveShip.isActive = false;
        currentActiveShip = null;
    }

    private void SelectNextShip(char direction)
    {
        if (currentActiveShip != null)
        {
            for (int i = 0; i < totalShipInSceneCount; i++)
            {
                if (currentActiveShip == shipClassifierScript[i])
                {
                    int nextShipIndex = CalculateNextShipIndex(direction,i);

                    currentActiveShip.isActive = false;
                    currentActiveShip = allShipsInScene[nextShipIndex].GetComponent<ShipClassifier>();
                    currentActiveShip.isActive = true;

                    activeShipIndex = nextShipIndex;

                    break;
                }
            }
        }
    }
    private int CalculateNextShipIndex(char direction, int i)
    {
        int nextShipIndex;
        if (direction == 'R')
        {
            nextShipIndex = (i + 1) % totalShipInSceneCount;
        }
        else//direction == 'L'
        {
            nextShipIndex = (i - 1 + totalShipInSceneCount) % totalShipInSceneCount;
        }
        return nextShipIndex;
    }

    private void EvaluateCameraPosition()
    {
        if (currentActiveShip != null)
        {
            if (currentActiveShip.GetComponent<ArrowShoot>() != null)
            {
                xOffset = 3;
                yOffset = 4;
            }
            else if (currentActiveShip.GetComponent<GunShoot>() != null)
            {
                xOffset = 8;
                yOffset = 10;
            }
            else//artillery ship
            {
                xOffset = 8;
                yOffset = 10;
            }

            transform.eulerAngles = new Vector3(55, -90, 0);

            float x = currentActiveShip.transform.position.x;
            float y = currentActiveShip.transform.position.y;
            float z = currentActiveShip.transform.position.z;

            transform.position = new Vector3(x + xOffset, y + yOffset, z + zOffset);
        }
        else
        {
            transform.position = new Vector3(defaultXCameraPos, defaultYCameraPos, defaultZCameraPos);
            transform.eulerAngles = new Vector3(defaultXCameraRot, defaultYCameraRot, defaultZCameraRot);
        }
    }
}
