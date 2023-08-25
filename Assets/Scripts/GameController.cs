using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject ShipsPrefab;
    private GameObject shipsPrefabInstance;
    private int totalBoatCount;
    private Ship[] ships;
    private BoatMovement[] shipMovers;
    private int selectedShipToPlay;

    
    private CameraFollower cameraFollower;
    private SmoothMover cameraMover;
    [SerializeField] private Vector3 cameraStartPosition;
    [SerializeField] private Vector3 cameraStartRotation;


    [SerializeField] private ObjectMarkerManager objectMarkerManager;
    [SerializeField] FixedJoystick joystick;

    [SerializeField] private Canvas selectionModeCanvas;
    [SerializeField] private Canvas objectMarkerCanvas;
    [SerializeField] private Canvas playModeCanvas;

    bool IsInPlayMode;  //is the current setup for playmode

    private void Start()
    {
        SetUpInitialScene();
    }

    private void SetUpInitialScene()
    {
        IsInPlayMode = false;
        SetUpShips();
        SetUpUI();
        SetUpCamera();
        MoveCameraToSelectedShip();
    }

    private void SetUpShips()
    {
        shipsPrefabInstance = Instantiate(ShipsPrefab);
        totalBoatCount = shipsPrefabInstance.transform.childCount;

        Transform[] shipObjects = new Transform[totalBoatCount];
        ships = new Ship[totalBoatCount];
        shipMovers = new BoatMovement[totalBoatCount];

        for (int i = 0; i < totalBoatCount; i++)
        {
            shipObjects[i] = shipsPrefabInstance.transform.GetChild(i);
            ships[i] = shipObjects[i].GetComponent<Ship>() as Ship;
            shipMovers[i] = shipObjects[i].GetComponent<BoatMovement>() as BoatMovement;
            shipMovers[i].joystickInput = joystick;
            shipMovers[i].canMove = false;  //dont move any ship
        }
        selectedShipToPlay = 0;  //first ship is by default selected
    }

    private void SetUpCamera()
    {
        Camera mainCam = Camera.main;
        if (mainCam.GetComponent<CameraFollower>() == null)
            mainCam.AddComponent<CameraFollower>();
        if (mainCam.GetComponent<SmoothMover>() == null)
            mainCam.AddComponent<SmoothMover>();
        cameraFollower = mainCam.GetComponent<CameraFollower>();
        cameraMover = mainCam.GetComponent<SmoothMover>();
        mainCam.transform.position = cameraStartPosition;
        mainCam.transform.eulerAngles = cameraStartRotation;
        cameraFollower.enabled = false;  //dont follow any ship/target 
    }

    private void SetUpUI()
    {
        selectionModeCanvas.gameObject.SetActive(true);
        objectMarkerCanvas.gameObject.SetActive(true);
        playModeCanvas.gameObject.SetActive(false);
        objectMarkerManager.objectMarkerCanvas = objectMarkerCanvas;
    }

    private void MoveCameraToSelectedShip()
    {
        
        Ship selection = ships[selectedShipToPlay];
        if (IsInPlayMode)
        {
            cameraFollower.target = selection.camTarget;
            cameraFollower.offset = selection.currentCameraOffset;
            cameraFollower.damping = selection.cameraDamping;
            Camera.main.transform.eulerAngles = selection.currentCameraRotation;
        }
        else
            cameraMover.MoveTo(selection.camTarget.position + selection.currentCameraOffset);
    }

    private void MoveSelectedShip()
    {
        foreach(BoatMovement mover in shipMovers)
            mover.canMove = false;
        shipMovers[selectedShipToPlay].canMove = true;
    }

    public void OnClickLeft()
    {
        selectedShipToPlay--;
        if(selectedShipToPlay < 0)
            selectedShipToPlay = totalBoatCount - 1;
        MoveCameraToSelectedShip();
        if(IsInPlayMode)
            MoveSelectedShip();
    }
    public void OnClickRight()
    {
        selectedShipToPlay = (selectedShipToPlay + 1) % totalBoatCount;
        MoveCameraToSelectedShip();
        if (IsInPlayMode)
            MoveSelectedShip();
    }

    public void OnClickPlay()
    {
        IsInPlayMode = true;
        playModeCanvas.gameObject.SetActive(true);
        selectionModeCanvas.gameObject.SetActive(false);
        objectMarkerCanvas.gameObject.SetActive(false);

        cameraFollower.enabled = true;
        MoveCameraToSelectedShip();
        MoveSelectedShip();
    }

    public void OnClickReset()
    {
        Destroy(shipsPrefabInstance);
        SetUpInitialScene();
    }
}
