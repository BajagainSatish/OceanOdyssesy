using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CameraFollowShip : MonoBehaviour
{
    private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3[] movementOffset = new Vector3[ButtonController.totalBoatCount];
    [SerializeField] private float damping = 0.5f;
    [SerializeField] private ButtonController buttonControllerScript;
    private Vector3 velocity = Vector3.zero;

    [Serializable]
    public struct StatinoaryOffsetCameraX
    {
        public float offsetValueX;
    }
    public StatinoaryOffsetCameraX[] offsetValuesX = new StatinoaryOffsetCameraX[ButtonController.totalBoatCount];

    [Serializable]
    public struct StatinoaryOffsetCameraY
    {
        public float offsetValueY;
    }
    public StatinoaryOffsetCameraY[] offsetValuesY = new StatinoaryOffsetCameraY[ButtonController.totalBoatCount];

    [Serializable]
    public struct StatinoaryOffsetCameraZ
    {
        public float offsetValueZ;
    }
    public StatinoaryOffsetCameraZ[] offsetValuesZ = new StatinoaryOffsetCameraZ[ButtonController.totalBoatCount];

    private void Start()
    {
        if (!buttonControllerScript.playIsClicked)
        {
            transform.eulerAngles = new Vector3(0, -135, 0);
        }
    }
    private void FixedUpdate()
    {
        if (!buttonControllerScript.playIsClicked)
        {
            for (int i = 0; i < ButtonController.totalBoatCount; i++)
            {
                if (buttonControllerScript.Ships[i].gameObject.activeSelf)
                {
                    transform.position = new Vector3(offsetValuesX[i].offsetValueX, offsetValuesY[i].offsetValueY, offsetValuesZ[i].offsetValueZ);
                }
            }
        }
        if (buttonControllerScript.playIsClicked)
        {
            target = buttonControllerScript.selectedShipToPlay;
            transform.eulerAngles = new Vector3(30, 30, 0);
            Vector3 movePosition = target.position + offset;
            transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);

            //values are experimental and can be re-adjusted
            if (target.name == "ShipA")
            {
                offset = movementOffset[0];//-43,90,-106
            }
            if (target.name == "ShipB")
            {
                offset = movementOffset[1];//-43,90,-106
            }
            if (target.name == "ShipC")
            {
                offset = movementOffset[2];//-43,90,-106
            }
            else if (target.name == "ShipD")
            {
                offset = movementOffset[3];//-43,71,-76
            }
            else if (target.name == "ShipE")
            {
                offset = movementOffset[4];//-18,40,-56
            }
            else if (target.name == "ShipF")
            {
                offset = movementOffset[5];//-43,71,-76
            }
            else if (target.name == "ShipG")
            {
                offset = movementOffset[6];//-22,43,-44
            }
        }
    }
}
/*
 	    {0,cameraOffsetX[0] },//Scale 1,1,1 : 25
        {1,cameraOffsetX[1] },//Scale 1,1,1 : 30
        {2,cameraOffsetX[2] },//Scale 1,1,1 : 27
        {3,cameraOffsetX[3] },//Scale 1,1,1 : 18
        {4,cameraOffsetX[4] },//Scale 1,1,1 : 8
        {5,cameraOffsetX[5] },//Scale 1,1,1 : 21
        {6,cameraOffsetX[6] }//Scale 1,1,1 : 10

        {0,cameraOffsetY[0] },//Scale 1,1,1 : 15
        {1,cameraOffsetY[1] },//Scale 1,1,1 : 15
        {2,cameraOffsetY[2] },//Scale 1,1,1 : 18
        {3,cameraOffsetY[3] },//Scale 1,1,1 : 10
        {4,cameraOffsetY[4] },//Scale 1,1,1 : 4
        {5,cameraOffsetY[5] },//Scale 1,1,1 : 12
        {6,cameraOffsetY[6] }//Scale 1,1,1 : 5

        {0,cameraOffsetZ[0] },//Scale 1,1,1 : 40
        {1,cameraOffsetZ[1] },//Scale 1,1,1 : 40
        {2,cameraOffsetZ[2] },//Scale 1,1,1 : 38
        {3,cameraOffsetZ[3] },//Scale 1,1,1 : 27
        {4,cameraOffsetZ[4] },//Scale 1,1,1 : 13
        {5,cameraOffsetZ[5] },//Scale 1,1,1 : 36
        {6,cameraOffsetZ[6] }//Scale 1,1,1 : 14

//scale 0.1
Offset Values X
2.5
2.85
2.95
1.63
0.88
1.94
1

Offset Values Y
1.33
1.75
1.72
0.91
0.4
1.05
0.47

Offset Values Z
4
4.25
4.28
2.95
1.3
3.69
1.56
//scale 0.1

 */ 