using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static int totalBoatCount = 7;
    public GameObject actualShipsPrefab;
    public Transform selectedShipToPlay;
    public Transform[] Ships = new Transform[totalBoatCount];//an array of 7 ships

    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject resetButton;
    [SerializeField] private GameObject joystick;
    public bool playIsClicked;
    private GameObject myShipPrefab;

    private void Awake()
    {
        myShipPrefab = Instantiate(actualShipsPrefab);
        for (int i = 0; i < totalBoatCount; i++)
        {
            Ships[i] = myShipPrefab.transform.GetChild(i);
            Ships[i].GetComponent<BoatMovement>().gameController = this;
            Ships[i].GetComponent<BoatMovement>().joystick = joystick.GetComponent<FixedJoystick>();
        }
    }

    private void Start()
    {
        joystick.SetActive(false);
        resetButton.SetActive(false);
        playIsClicked = false;
        Ships[0].gameObject.SetActive(true);
        Ships[0].GetChild(3).gameObject.SetActive(false);
        for (int i = 1; i < totalBoatCount; i++)
        {
            Ships[i].gameObject.SetActive(false);
            Ships[i].GetChild(3).gameObject.SetActive(false);//disable particle effect for all ships
        }
    }
    public void OnClickLeft()
    {
        for (int i = 1; i < totalBoatCount; i++)//omit the first boat
        {
            if (Ships[i].gameObject.activeInHierarchy == true)
            {
                Ships[i].gameObject.SetActive(false);
                Ships[i - 1].gameObject.SetActive(true);
                return;
            }
        }
    }
    public void OnClickRight()
    {
        for (int i = 0; i < totalBoatCount - 1; i++)//omit the last boat
        {
            if (Ships[i].gameObject.activeInHierarchy == true)
            {
                Ships[i].gameObject.SetActive(false);
                Ships[i + 1].gameObject.SetActive(true);
                return;
            }
        }
    }

    public void OnClickPlay()
    {
        leftButton.SetActive(false);
        rightButton.SetActive(false);
        playButton.SetActive(false);
        joystick.SetActive(true);
        resetButton.SetActive(true);
        selectedShipToPlay = ActiveShip();
        selectedShipToPlay.GetChild(3).gameObject.SetActive(true);
        playIsClicked = true;
    }

    public void OnClickReset()
    {
        Destroy(myShipPrefab);
        Awake();
        leftButton.SetActive(true);
        rightButton.SetActive(true);
        playButton.SetActive(true);
        Start();
    }

    private Transform ActiveShip()
    {
        //Return the one ship that is currently active
        for (int i = 0; i < totalBoatCount; i++)
        {
            if (Ships[i].gameObject.activeInHierarchy)
            {
                return Ships[i].transform;
            }
        }
        return null;
    }
}
