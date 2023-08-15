using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public static int totalBoatCount = 7;
    public Transform[] Ships = new Transform[totalBoatCount];//an array of 7 ships
    public Transform selectedShipToPlay;

    [SerializeField] private GameObject leftButton;
    [SerializeField] private GameObject rightButton;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject joystick;
    public bool playIsClicked;

    private void Start()
    {
        joystick.SetActive(false);
        playIsClicked = false;
        Ships[0].gameObject.SetActive(true);
        Ships[0].GetChild(2).gameObject.SetActive(false);
        for (int i = 1; i < totalBoatCount; i++)
        {
            Ships[i].gameObject.SetActive(false);
            Ships[i].GetChild(2).gameObject.SetActive(false);
        }
    }
    public void OnClickLeft()
    {
        for (int i = 1; i < totalBoatCount; i++)//omit the first boat
        {
            if (Ships[i].gameObject.activeSelf == true)
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
            if (Ships[i].gameObject.activeSelf == true)
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
        selectedShipToPlay = ActiveShip();
        selectedShipToPlay.GetChild(2).gameObject.SetActive(true);
        playIsClicked = true;
    }

    private Transform ActiveShip()
    {
        //Return the one ship that is currently active
        for (int i = 0; i < totalBoatCount; i++)
        {
            if (Ships[i].gameObject.activeSelf)
            {
                return Ships[i].transform;
            }
        }
        return null;
    }
}
