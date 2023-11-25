using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAlignTowardsEnemy : MonoBehaviour
{
    public Transform target;
    private float speed;
    
    private ShipCategorizer_Level shipCategorizer_LevelScript;
    private CannonShoot cannonShootScript;

    private void Awake()
    {
        shipCategorizer_LevelScript = GetComponent<ShipCategorizer_Level>();
        cannonShootScript = GetComponent<CannonShoot>();
    }
    private void Start()
    {
        if (shipCategorizer_LevelScript.shipLevel == ShipCategorizer_Level.ShipLevels.Level1)
        {
            AssignValue(0);
        }
        else if (shipCategorizer_LevelScript.shipLevel == ShipCategorizer_Level.ShipLevels.Level2)
        {
            AssignValue(1);
        }
        else if (shipCategorizer_LevelScript.shipLevel == ShipCategorizer_Level.ShipLevels.Level3)
        {
            AssignValue(2);
        }
        else if (shipCategorizer_LevelScript.shipLevel == ShipCategorizer_Level.ShipLevels.Level4)
        {
            AssignValue(3);
        }
    }
    private void Update()
    {
        if (cannonShootScript.targetEnemy != null)
        {
            target = cannonShootScript.targetEnemy;
        }
        else
        {
            target = null;
        }

        if (target != null)
        {
            Vector3 toTarget = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(toTarget);

            // Calculate the angle between the ship's forward and backward directions
            float angleForward = Quaternion.Angle(transform.rotation, rotation);
            float angleBackward = Quaternion.Angle(transform.rotation * Quaternion.Euler(0, 180, 0), rotation);

            // Choose the rotation that requires less turning
            Quaternion finalRotation = (angleBackward < angleForward) ? Quaternion.Euler(0, 180, 0) * rotation : rotation;

            // Apply the rotation gradually
            transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation, speed * Time.deltaTime);

            // Optional: Adjust the local Euler angles if needed
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }       
    }
    private void AssignValue(int index)
    {
        speed = SetParameters.shipRotationSpeed[index];
    }
}

