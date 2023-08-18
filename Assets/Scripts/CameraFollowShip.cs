using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CameraFollowShip : MonoBehaviour
{
    private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3[] movementOffset = new Vector3[GameController.totalBoatCount];
    [SerializeField] private float damping = 0.5f;
    [SerializeField] private GameController GameControllerScript;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Vector3 ShipSelectionRotation = new Vector3(0, -135, 0);
    [SerializeField] private Vector3 ShipPlayRotation = new Vector3(30,30, 0);

    [Serializable]
    public struct StatinoaryOffsetCameraX
    {
        public float offsetValueX;
    }
    public StatinoaryOffsetCameraX[] offsetValuesX = new StatinoaryOffsetCameraX[GameController.totalBoatCount];

    [Serializable]
    public struct StatinoaryOffsetCameraY
    {
        public float offsetValueY;
    }
    public StatinoaryOffsetCameraY[] offsetValuesY = new StatinoaryOffsetCameraY[GameController.totalBoatCount];

    [Serializable]
    public struct StatinoaryOffsetCameraZ
    {
        public float offsetValueZ;
    }
    public StatinoaryOffsetCameraZ[] offsetValuesZ = new StatinoaryOffsetCameraZ[GameController.totalBoatCount];

    private void FixedUpdate()
    {
        if (!GameControllerScript.playIsClicked)
        {
            transform.eulerAngles = ShipSelectionRotation;
            for (int i = 0; i < GameController.totalBoatCount; i++)
            {
                if (GameControllerScript.Ships[i].gameObject.activeSelf)
                {
                    transform.position = new Vector3(offsetValuesX[i].offsetValueX, offsetValuesY[i].offsetValueY, offsetValuesZ[i].offsetValueZ);
                }
            }
        }
        if (GameControllerScript.playIsClicked)
        {
            target = GameControllerScript.selectedShipToPlay;
            transform.eulerAngles = ShipPlayRotation;
            Vector3 movePosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);

            //values are experimental and can be re-adjusted
            if (target.name == "ShipA")
            {
                offset = movementOffset[0];//-43,90,-106
                damping = 0.5f;
            }
            if (target.name == "ShipB")
            {
                offset = movementOffset[1];//-43,90,-106
                damping = 0.5f;
            }
            if (target.name == "ShipC")
            {
                offset = movementOffset[2];//-43,90,-106
                damping = 0.4f;
            }
            else if (target.name == "ShipD")
            {
                offset = movementOffset[3];//-43,71,-76
                damping = 0.3f;
            }
            else if (target.name == "ShipE")
            {
                offset = movementOffset[4];//-18,40,-56
                damping = 0.1f;
            }
            else if (target.name == "ShipF")
            {
                offset = movementOffset[5];//-43,71,-76
                damping = 0.3f;
            }
            else if (target.name == "ShipG")
            {
                offset = movementOffset[6];//-22,43,-44
                damping = 0.5f;
            }
        }
    }
}