using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SetParameters
{
    //Common to all 4 attacking ships
    public static int mediumShipMenCount = 4;

    //Archer Values
    public static float archerLineWidth = 0.01f;
    public static float archerArrowVelocity = 2.5f;
    public static float archersleastDistanceForStraightHit = 2f;
    public static float archerAdjustCurveAngle = 0.7f;
    public static float archer_WaitBeforeShoot_FirstEncounter = 2f;
    public static float archer_WaitBeforeShoot_Aiming = 2f;
    public static float archer_WaitAfterShoot = 2f;

    //Total shoot time = archer_WaitBeforeShoot_Aiming + archer_WaitAfterShoot

    //Gunmen Values
    public static float gunmanLineWidth = 0.01f;
    public static float gunmanBulletVelocity = 10f;
    public static float gunman_WaitBeforeShoot_FirstEncounter = 2f;
    public static float gunman_WaitBeforeShoot_Aiming = 2f;
    public static float gunman_WaitAfterShoot = 2f;

    //Cannon Values
    public static float cannonLineWidth = 0.02f;
    public static float cannonBallVelocity = 10f;
    public static float cannonShootAngleRange = 55f;
    public static float cannon_WaitBeforeShoot_FirstEncounter = 4f;
    public static float cannon_WaitBeforeShoot_Aiming = 4f;
    public static float cannon_WaitAfterShoot = 4f;

    //Mortar Values
    public static float mortarLineWidth = 0.07f;
    public static float mortarBombVelocity = 5f;
    public static float mortarAdjustCurveAngle = -0.7f;
    public static float mortar_WaitBeforeShoot_FirstEncounter = 4f;
    public static float mortar_WaitBeforeShoot_Aiming = 4f;
    public static float mortar_WaitAfterShoot = 4f;

    //Common Archer and Mortar Values
    public static int curvePointsTotalCount = 20;//change this value to change the number of points in curve, and control smoothness of curve by increasing the number

    //Ship Attributes on basis of Levels
    public static float[] archerWeaponRange = new float[] { 3f, 5f, 7f, 10f };
    public static float[] cannonWeaponRange = new float[] { 3f, 5f, 7f, 10f };
    public static float[] gunmanWeaponRange = new float[] { 3f, 5f, 7f, 10f };
    public static float[] mortarWeaponRange = new float[] { 3f, 5f, 7f, 10f };

    public static float[] shipHealth = new float[] {160f,220f,280f,340f};
    public static float[] shipSpeed = new float[] {1.05f,1.10f,1.15f,1.20f};
    public static int[] shipCost = new int[] {80,120,160,200};

    public static float[] weaponDamage = new float[] {7f,22f,25f,20f};
    public static float[] weaponReloadSpeed = new float[] {0.8f, 0.7f, 0.6f, 0.5f};
    public static int[] weaponMaxAmmo = new int[] {30,50,60,70};
}
