using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCategorizer_Level : MonoBehaviour
{
    public enum ShipLevels
    {
        Level1, Level2, Level3, Level4
    };
    public ShipLevels shipLevel;

    public int shipHealth;
    private float shipSpeed;
    private int shipCost;

    public float weaponRange;
    public int weaponDamage;
    //weapon max ammo and weapon reload speed handled in other scripts

    private TargetingSystem_PhysicsOverlapSphere targetingSystem_PhysicsOverlapSphereScript;

    //Move this code to update or another approach if level of ship upgrades within game
    private void Awake()
    {
        targetingSystem_PhysicsOverlapSphereScript = GetComponent<TargetingSystem_PhysicsOverlapSphere>();

        if (shipLevel == ShipLevels.Level1)
        {
            AssignValue(0);
        }
        else if (shipLevel == ShipLevels.Level2)
        {
            AssignValue(1);
        }
        else if (shipLevel == ShipLevels.Level3)
        {
            AssignValue(2);
        }
        else if (shipLevel == ShipLevels.Level4)
        {
            AssignValue(3);
        }
    }

    private void AssignValue(int index)
    {
        if (targetingSystem_PhysicsOverlapSphereScript.thisShipType == TargetingSystem_PhysicsOverlapSphere.ShipType.ArcherShip)
        {
            weaponRange = SetParameters.archerWeaponRange[index];
            shipHealth = SetParameters.archerShipHealth[index];
            weaponDamage = SetParameters.archerWeaponDamage[index];
        }
        else if (targetingSystem_PhysicsOverlapSphereScript.thisShipType == TargetingSystem_PhysicsOverlapSphere.ShipType.CannonShip)
        {
            weaponRange = SetParameters.cannonWeaponRange[index];
            shipHealth = SetParameters.cannonShipHealth[index];
            weaponDamage = SetParameters.cannonWeaponDamage[index];
        }
        else if (targetingSystem_PhysicsOverlapSphereScript.thisShipType == TargetingSystem_PhysicsOverlapSphere.ShipType.GunmanShip)
        {
            weaponRange = SetParameters.gunmanWeaponRange[index];
            shipHealth = SetParameters.gunmanShipHealth[index];
            weaponDamage = SetParameters.gunmanWeaponDamage[index];
        }
        else if (targetingSystem_PhysicsOverlapSphereScript.thisShipType == TargetingSystem_PhysicsOverlapSphere.ShipType.MortarShip)
        {
            weaponRange = SetParameters.mortarWeaponRange[index];
            shipHealth = SetParameters.mortarShipHealth[index];
            weaponDamage = SetParameters.mortarWeaponDamage[index];
        }

        shipSpeed = SetParameters.shipSpeed[index];
        shipCost = SetParameters.shipCost[index];
    }
}
