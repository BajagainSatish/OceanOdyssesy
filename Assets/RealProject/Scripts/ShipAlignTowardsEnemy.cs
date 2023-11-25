using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAlignTowardsEnemy : MonoBehaviour
{
    public Transform target;
    private float speed;
    
    private ShipCategorizer_Level shipCategorizer_LevelScript;
    private CannonShoot cannonShootScript;
    private ArcherShoot archerShootScript;
    private GunShoot gunShootScript;

    private string thisShipType;

    private void Awake()
    {
        shipCategorizer_LevelScript = GetComponent<ShipCategorizer_Level>();
        if (TryGetComponent<CannonShoot>(out _))
        {
            cannonShootScript = GetComponent<CannonShoot>();
            thisShipType = "CannonShip";
        }
        else if (TryGetComponent<ArcherShoot>(out _))
        {
            archerShootScript = GetComponent<ArcherShoot>();
            thisShipType = "ArcherShip";
        }
        else if (TryGetComponent<GunShoot>(out _))
        {
            gunShootScript = GetComponent<GunShoot>();
            thisShipType = "GunmanShip";
        }
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
        if (thisShipType == "CannonShip" && cannonShootScript.targetEnemy != null)
        {
            target = cannonShootScript.targetEnemy;
        }
        else if (thisShipType == "ArcherShip" && archerShootScript.targetEnemy != null)
        {
            target = archerShootScript.targetEnemy;
        }
        else if (thisShipType == "GunmanShip" && gunShootScript.targetEnemy != null)
        {
            target = gunShootScript.targetEnemy;
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

            // Adjust the local Euler angles to ensure ship doesnot rotate along x axis.
            transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }       
    }
    private void AssignValue(int index)
    {
        speed = SetParameters.shipRotationSpeed[index];
    }
}

