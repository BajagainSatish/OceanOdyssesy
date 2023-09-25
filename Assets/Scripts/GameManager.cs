using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


[RequireComponent(typeof(MoveShipsAlongPathsManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject ShipsCollectionParentPrefab;  //the parent object containing the collection of ships
    private GameObject ShipsCollectionParent;
    private GameObject[] ShipsCollection;   //the whole collection of ships to select from
    [SerializeField] private string shipObjectTag = "Ship"; //used to isentify ship object among the children of parent object of the collection

    Camera mainCam;
    private CameraFollower cameraFollower;
    private CameraRotateAround cameraRotateHandler;
    private CameraZoom cameraZoomHandler;

    MoveShipsAlongPathsManager moveShipsAlongPathsManager;
    [SerializeField] int noOfShipsToSelect;
    private Ship[] selectedShips;
    private int selectedMovementShip;  //the index of selcetdShips array which is currently being used as target of camera follower

    private enum GameMode { CANON_FIRE, SHIP_MOVE};
    private GameMode gameMode;

    public Button playButton;
    public Button stopButton;
    public Button fireButton;

    private bool[] isShipMoving;  //each ship is moving or not

    public CameraFollowerTarget canonShipCameraTarget;   //use this to hold values for camera rotation and position for canon ship

    public GameObject[] canonShooterActiveMarkers;  //the gameobject that indicates which objects is currently selected
    private int activeCanonShooter; //which cannon shooter will fire on button press
    public AnimationStateController[] canonShooters;  //the player objects that will shoot canon


    private void Start()
    {
        Random.InitState((int)(Time.realtimeSinceStartup * 100));  //initialize random number generator such that it gives different result every play

        moveShipsAlongPathsManager = GetComponent<MoveShipsAlongPathsManager>();
        gameMode = GameMode.CANON_FIRE;

        SetCameraComponents();
        SetUpInitialScene();
    }

    private void SetCameraComponents()
    {
        mainCam = Camera.main;
        if (mainCam.GetComponent<CameraFollower>() == null)
            mainCam.gameObject.AddComponent<CameraFollower>();
        if (mainCam.GetComponent<CameraRotateAround>() == null)
            mainCam.gameObject.AddComponent<CameraRotateAround>();
        if (mainCam.GetComponent<CameraZoom>() == null)
            mainCam.gameObject.AddComponent<CameraZoom>();

        cameraFollower = mainCam.GetComponent<CameraFollower>();
        cameraRotateHandler = mainCam.GetComponent<CameraRotateAround>();
        cameraZoomHandler = mainCam.GetComponent<CameraZoom>();
    }

    private void SetUpInitialScene()
    {
        SetUpShips();
        SetUpShipsInPath();

        selectedMovementShip = 0;
        SetSceneBasedOnGameMode();
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
        isShipMoving = new bool[noOfShipsToSelect];
        for (int i = 0; i < _selectedShips.Length; i++)
        {
            selectedShips[i] = _selectedShips[i].GetComponent<Ship>();
            moveShipsAlongPathsManager.SetMovementSpeed(i, selectedShips[i].speed);
            isShipMoving[i] = false;
        }
    }

    private void SetSceneBasedOnGameMode()
    {
        SetCamera();
        SetUI();
        if(gameMode == GameMode.CANON_FIRE)
        {
            //set player indicator for active shooter
            SetActiveCanonShooterIndicator();
        }
    }
    private void SetCamera()
    {
        if (gameMode == GameMode.CANON_FIRE)
        {
            cameraRotateHandler.enabled = true;  //allow zooming and rotation around the ship
            cameraZoomHandler.enabled = true;
            cameraRotateHandler.target = canonShipCameraTarget.camTarget;
            cameraZoomHandler.target = canonShipCameraTarget.camTarget;
            cameraFollower.enabled = false;

            Vector3 newCampos = canonShipCameraTarget.camTarget.position + canonShipCameraTarget.cameraOffset;
            SmoothObjectMovement.MoveObjectTo(mainCam.gameObject, newCampos);
            Quaternion newCamRot = canonShipCameraTarget.cameraRotation;
            SmoothObjectMovement.RotateObjectTo(mainCam.gameObject, newCamRot);
            //mainCam.transform.rotation = newCamRot;
        }
        else
        {
            cameraRotateHandler.enabled = false;
            cameraZoomHandler.enabled = false;
            cameraFollower.enabled = true;

            cameraFollower.target = selectedShips[selectedMovementShip].cameraTarget;
            Quaternion newCamRot = cameraFollower.target.cameraRotation;
            SmoothObjectMovement.RotateObjectTo(mainCam.gameObject, newCamRot);
            cameraFollower.enabled = true;
        }
    }
    private void SetUI()
    {
        if (gameMode == GameMode.CANON_FIRE)
        {
            playButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(false);
            fireButton.gameObject.SetActive(true); //only fire button is active while focusing on canon ship
        }
        else
        {
            if (isShipMoving[selectedMovementShip])
            {
                playButton.gameObject.SetActive(false);
                stopButton.gameObject.SetActive(true);
            }
            else
            {
                playButton.gameObject.SetActive(true);
                stopButton.gameObject.SetActive(false);
            }
            
            fireButton.gameObject.SetActive(false);
        }
    }
    private void SetActiveCanonShooterIndicator()
    {
        if (canonShooterActiveMarkers == null || canonShooterActiveMarkers.Length != canonShooters.Length)
            return;
        foreach(GameObject indicatorObject in canonShooterActiveMarkers)
            indicatorObject.SetActive(false);
        canonShooterActiveMarkers[activeCanonShooter].SetActive(true);
    }

    
    public void OnClickPlay()
    {
        moveShipsAlongPathsManager.MoveShip(selectedMovementShip);
        isShipMoving[selectedMovementShip] = true;
        SetUI();
    }
    public void OnClickPause()
    {
        moveShipsAlongPathsManager.StopShip(selectedMovementShip);
        isShipMoving[selectedMovementShip] = false;
        SetUI();
    }

    public void OnClickLeftArrow()
    {
        if (gameMode == GameMode.SHIP_MOVE)
        {
            selectedMovementShip--;
            if (selectedMovementShip < 0)
                selectedMovementShip = noOfShipsToSelect - 1;

            SetCamera();
            SetUI();  //pause play button might need to be enabled/disabled
        }
        else
        {
            activeCanonShooter--;
            if(activeCanonShooter < 0)
                activeCanonShooter = canonShooters.Length - 1;
            SetActiveCanonShooterIndicator();
        }
        
    }
    public void OnClickRightArrow()
    {
        if (gameMode == GameMode.SHIP_MOVE)
        {
            selectedMovementShip = (selectedMovementShip + 1) % noOfShipsToSelect;
            SetCamera();
            SetUI();  //pause play button might need to be enabled/disabled
        }
        else
        {
            activeCanonShooter = (activeCanonShooter + 1) % canonShooters.Length;
            SetActiveCanonShooterIndicator();
        }
    }

    public void OnClickGameMode()
    {
        if(gameMode == GameMode.CANON_FIRE)
            gameMode = GameMode.SHIP_MOVE;
        else
            gameMode = GameMode.CANON_FIRE;
        SetSceneBasedOnGameMode();
    }

    public void OnClickShoot()
    {
        //fire respective canon
        canonShooters[activeCanonShooter].selectedUnitToShoot = true;
    }

    public void OnClickReset()
    {
        Destroy(ShipsCollectionParent);
        SetUpInitialScene();
    }


    private void LateUpdate()
    { 
        if (gameMode == GameMode.CANON_FIRE)
        {//update the camera offset and rotation
            canonShipCameraTarget.SetFromCurrentCameraPosition();
        }
    }
}
