using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject ShipsPrefab;
    private GameObject shipsPrefabInstance;
    private int totalBoatCount;
    private Ship[] ships;
    private BoatMovement[] shipMovers;
    private int selectedShipToPlay;
    private bool shipIsInFocus;

    Camera mainCam;
    private CameraFollower cameraFollower;
    private SmoothMover cameraMover;
    private CameraRotateAround cameraRotateHandler;
    private CameraZoom cameraZoomHandler;
    private CameraPanning cameraPanningHandler;
    [SerializeField] private Vector3 cameraStartPosition;
    [SerializeField] private Vector3 cameraStartRotation;
    [SerializeField] private Transform dummyCameraTarget;  //use as a subtitute for actual focus object so as to handle panning seamlessly


    [SerializeField] private ObjectMarkerManager objectMarkerManager;
    [SerializeField] FixedJoystick joystick;

    [SerializeField] private Canvas selectionModeCanvas;
    [SerializeField] private Canvas objectMarkerCanvas;
    [SerializeField] private Canvas playModeCanvas;
    [SerializeField] private Button cameraPanButton;
    [SerializeField] private Button cameraRotateButton;

    bool IsInPlayMode;  //is the current setup for playmode

    private void Start()
    {
        SetCameraComponents();
        SetUpInitialScene();
    }

    private void SetCameraComponents()
    {
        mainCam = Camera.main;
        if (mainCam.GetComponent<CameraFollower>() == null)
            mainCam.AddComponent<CameraFollower>();
        if (mainCam.GetComponent<SmoothMover>() == null)
            mainCam.AddComponent<SmoothMover>();
        if (mainCam.GetComponent<CameraRotateAround>() == null)
            mainCam.AddComponent<CameraRotateAround>();
        if (mainCam.GetComponent<CameraZoom>() == null)
            mainCam.AddComponent<CameraZoom>();
        if (mainCam.GetComponent<CameraPanning>() == null)
            mainCam.AddComponent<CameraPanning>();
        cameraFollower = mainCam.GetComponent<CameraFollower>();
        cameraMover = mainCam.GetComponent<SmoothMover>();
        cameraRotateHandler = mainCam.GetComponent<CameraRotateAround>();
        cameraZoomHandler = mainCam.GetComponent<CameraZoom>();
        cameraPanningHandler = mainCam.GetComponent<CameraPanning>();
        cameraRotateHandler.target = dummyCameraTarget;
        cameraZoomHandler.target = dummyCameraTarget;
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
            ships[i].id = i;
            shipMovers[i] = shipObjects[i].GetComponent<BoatMovement>() as BoatMovement;
            shipMovers[i].joystickInput = joystick;
            shipMovers[i].canMove = false;  //dont move any ship
        }
        selectedShipToPlay = 0;  //first ship is by default selected
    }

    private void SetUpCamera()
    {
        mainCam.transform.position = cameraStartPosition;
        mainCam.transform.eulerAngles = cameraStartRotation;
        cameraFollower.enabled = false;  //dont follow any ship/target 
        cameraRotateHandler.enabled = false;  //first disable rotation //camera panning and roation have same gesture control so only one can be enabled at a time
        cameraZoomHandler.enabled = true;
        cameraPanningHandler.enabled = true;  //first enable panning 
    }

    private void SetUpUI()
    {
        selectionModeCanvas.gameObject.SetActive(true);
        objectMarkerCanvas.gameObject.SetActive(true);
        playModeCanvas.gameObject.SetActive(false);
        objectMarkerManager.objectMarkerCanvas = objectMarkerCanvas;
        OnClickCameraPanning();
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
        dummyCameraTarget.position = selection.camTarget.position;
        shipIsInFocus = true;
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
        cameraRotateHandler.enabled = false;
        cameraZoomHandler.enabled = false;
        cameraPanningHandler.enabled = false;
        MoveCameraToSelectedShip();
        MoveSelectedShip();
    }

    public void OnClickReset()
    {
        Destroy(shipsPrefabInstance);
        SetUpInitialScene();
    }

    public void OnClickCameraPanning()
    {//change from camera panning to rotation
        cameraPanningHandler.enabled = true;
        cameraRotateHandler.enabled = false;
        cameraPanButton.gameObject.SetActive(false);
        cameraRotateButton.gameObject.SetActive(true);
    }
    public void OnClickCameraRotation()
    {//change from camera rotation to panning
        cameraRotateHandler.enabled = true;
        cameraPanningHandler.enabled = false;
        cameraRotateButton.gameObject.SetActive(false);
        cameraPanButton.gameObject.SetActive(true);
    }

    public void OnFocusableObjectFocus(FocusableObject focusableObject)
    {
        dummyCameraTarget.position = focusableObject.target.position;
        if (focusableObject.tag == "Ship")
        {
            Ship focusedShip = focusableObject.GetComponent<Ship>();
            selectedShipToPlay = focusedShip.id;
            MoveCameraToSelectedShip();
            shipIsInFocus = true;
        }
        else
        {
            cameraMover.MoveTo(focusableObject.GetFocusTargetPosition(Camera.main.transform.position));
            shipIsInFocus = false;
        }
    }

    private void LateUpdate()
    {
        if (cameraPanningHandler.isPanning)  
            dummyCameraTarget.SetParent(mainCam.transform);  //the dummy target's position should remain constant relative to camera unless changed specifically
        else
            dummyCameraTarget.SetParent(null); //while zooming camera will move towards target which will cause the dummyTarget to also move away if camera is set as parent

        if(shipIsInFocus)
        {
            ships[selectedShipToPlay].SetFromCurrentCameraPosition();
        }
    }
}
