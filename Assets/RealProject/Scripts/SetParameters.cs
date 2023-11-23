using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SetParameters
{
    //Common to all 4 attacking ships
    public static int mediumShipMenCount = 4;
    public static float levelSpecificWeaponRange = 5f;

    //Archer Values
    public static float archerLineWidth = 0.01f;
    public static float archerArrowVelocity = 2.5f;
    public static float archersleastDistanceForStraightHit = 2f;
    public static float archerAdjustCurveAngle = 0.7f;
    public static float archer_WaitBeforeShoot_FirstEncounter = 4f;
    public static float archer_WaitBeforeShoot_Aiming = 4f;
    public static float archer_WaitAfterShoot = 4f;

    //Total shoot time = archer_WaitBeforeShoot_Aiming + archer_WaitAfterShoot

    //Gunmen Values
    public static float gunmanLineWidth = 0.01f;
    public static float gunmanBulletVelocity = 10f;
    public static float gunmanCoolDownTime = 5f;

    //Cannon Values
    public static float cannonLineWidth = 0.02f;
    public static float cannonBallVelocity = 10f;
    public static float cannonCoolDownTime = 10f;
    public static float cannonShootAngleRange = 55f;

    //Mortar Values
    public static float mortarLineWidth = 0.07f;
    public static float mortarBombVelocity = 5f;
    public static float mortarCoolDownTime = 5f;
    public static float mortarAdjustCurveAngle = -0.7f;

    //Common Archer and Mortar Values
    public static int curvePointsTotalCount = 20;//change this value to change the number of points in curve, and control smoothness of curve by increasing the number
}
