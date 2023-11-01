using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryDemo : MonoBehaviour
{
    [SerializeField] private static int totalShipInSceneCount = 14;

    [SerializeField] private GameObject[] allShipsInScene = new GameObject[totalShipInSceneCount];
    private ShipClassifier[] shipClassifierScript = new ShipClassifier[totalShipInSceneCount];
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
        //For now set isActive = true manually in inspector, later will implemented by tap on ship
        //at a time, only one can be true, no more than one can be true
        //when any one is set to true, set all remaining others to false

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentActiveShip == null)
            {
                currentActiveShip = allShipsInScene[activeShipIndex].GetComponent<ShipClassifier>();
                currentActiveShip.isActive = true;
            }
            else if (currentActiveShip != null)
            {
                currentActiveShip.isActive = false;
                currentActiveShip = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentActiveShip != null)
            {
                for (int i = 0; i < totalShipInSceneCount; i++)
                {
                    if (currentActiveShip == shipClassifierScript[i])
                    {
                        int nextShipIndex = (i + 1) % totalShipInSceneCount;

                        currentActiveShip.isActive = false;
                        currentActiveShip = allShipsInScene[nextShipIndex].GetComponent<ShipClassifier>();
                        currentActiveShip.isActive = true;

                        activeShipIndex = nextShipIndex;

                        break;
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentActiveShip != null)
            {
                for (int i = 0; i < totalShipInSceneCount; i++)
                {
                    if (currentActiveShip == shipClassifierScript[i])
                    {
                        int nextShipIndex = (i - 1 + totalShipInSceneCount) % totalShipInSceneCount;

                        currentActiveShip.isActive = false;
                        currentActiveShip = allShipsInScene[nextShipIndex].GetComponent<ShipClassifier>();
                        currentActiveShip.isActive = true;

                        activeShipIndex = nextShipIndex;

                        break;
                    }
                }
            }
        }

        //Temporary Camera Control
        if (currentActiveShip != null)
        {
            if (currentActiveShip.GetComponent<ArrowShoot>() != null)
            {
                xOffset = 3;
                yOffset = 4;
            }
            else if (currentActiveShip.GetComponent<GunShoot>() != null)
            {
                xOffset = 4;
                yOffset = 6;
            }
            else//artillery ship
            {
                xOffset = 8;
                yOffset = 10;
            }

            this.transform.eulerAngles = new Vector3(55, -90, 0);
            this.transform.position = new Vector3(currentActiveShip.transform.position.x + xOffset, currentActiveShip.transform.position.y + yOffset, currentActiveShip.transform.position.z + zOffset);
        }
        else
        {
            this.transform.position = new Vector3(defaultXCameraPos, defaultYCameraPos, defaultZCameraPos);
            this.transform.eulerAngles = new Vector3(defaultXCameraRot, defaultYCameraRot, defaultZCameraRot);
        }

        //Arrow marker control, when no ship is selected, deactivate all markers
        if (currentActiveShip == null)
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
    }
}
