using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Animations;

[RequireComponent(typeof(MoveShipsAlongPathsManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ShipsCollectionParentPrefab;  //the parent object containing the collection of ships
    private GameObject ShipsCollectionParent;
    private GameObject[] ShipsCollection;   //the whole collection of ships to select from
    [SerializeField] private string shipObjectTag = "Ship"; //used to isentify ship object among the children of parent object of the collection

    Camera mainCam;
    private CameraFollower cameraFollower;

    MoveShipsAlongPathsManager moveShipsAlongPathsManager;
    [SerializeField] int noOfShipsToSelect;
    private Ship[] selectedShips;
    private int cameraFollowerTargetShip;  //the index of selcetdShips array which is currently being used as target of camera follower


    private void Start()
    {
        moveShipsAlongPathsManager = GetComponent<MoveShipsAlongPathsManager>();

        SetCameraComponents();
        SetUpInitialScene();
    }

    private void SetCameraComponents()
    {
        mainCam = Camera.main;
        if (mainCam.GetComponent<CameraFollower>() == null)
            mainCam.gameObject.AddComponent<CameraFollower>();

        cameraFollower = mainCam.GetComponent<CameraFollower>();
    }

    private void SetUpInitialScene()
    {
        SetUpShips();
        SetUpShipsInPath();

        cameraFollowerTargetShip = 0;
        SetCameraTarget();
        SetUpUI();
    }
    private void SetUpShips()
    {
        ShipsCollectionParent = Instantiate(ShipsCollectionParentPrefab); //make a copy of ships so that it is easy to reset the scene
        ShipsCollectionParent.SetActive(true);  //if not active set active in scene

        List<GameObject> shipsCollection = new List<GameObject>();

        for (int i = 0; i < ShipsCollectionParent.transform.childCount; i++)
        {
            Transform childObject = ShipsCollectionParent.transform.GetChild(i);
            if (childObject.tag == shipObjectTag)
            {
                shipsCollection.Add(childObject.gameObject);
            }
        }
        ShipsCollection = shipsCollection.ToArray();
    }
    private void SetUpShipsInPath()
    {
        moveShipsAlongPathsManager.SetShipsToSelectFrom(ShipsCollection, noOfShipsToSelect);
        moveShipsAlongPathsManager.Initialize();
        GameObject[] _selectedShips = moveShipsAlongPathsManager.GetSelectedShips();
        noOfShipsToSelect = _selectedShips.Length; //might be different if entered out of bound e.g. if there are only 2 ships in collection but noofships to selcet is entered as 3
        selectedShips = new Ship[noOfShipsToSelect];
        for (int i = 0; i < _selectedShips.Length; i++)
        {
            selectedShips[i] = _selectedShips[i].GetComponent<Ship>();
            moveShipsAlongPathsManager.SetMovementSpeed(i, selectedShips[i].speed);
        }
    }
    private void SetCameraTarget()
    {
        cameraFollower.target = selectedShips[cameraFollowerTargetShip].cameraTarget;
        mainCam.transform.rotation = cameraFollower.target.cameraRotation;
    }
    private void SetUpUI()
    {
      
    }

    public void OnClickPlay()
    {
        moveShipsAlongPathsManager.MoveShip(cameraFollowerTargetShip);
    }
    public void OnClickPause()
    {
        moveShipsAlongPathsManager.StopShip(cameraFollowerTargetShip);
    }

    public void OnClickLeftArrow()
    {
        cameraFollowerTargetShip--;
        if (cameraFollowerTargetShip < 0)
            cameraFollowerTargetShip = noOfShipsToSelect - 1;
        SetCameraTarget();
    }
    public void OnClickRightArrow()
    {
        cameraFollowerTargetShip = (cameraFollowerTargetShip + 1) % noOfShipsToSelect;
        SetCameraTarget();
    }

    public void OnClickReset()
    {
        Destroy(ShipsCollectionParent);
        SetUpInitialScene();
    }
}
