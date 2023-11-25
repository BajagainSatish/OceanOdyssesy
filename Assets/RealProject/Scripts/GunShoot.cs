using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ShipCategorizer_Level;

public class GunShoot : MonoBehaviour
{
    [SerializeField] private ObjectPool_Projectile objectPoolBulletScript;
    private float lineWidth;
    private float bulletVelocity;
    private float gunmanMaxRange;
    private float waitBeforeShoot_FirstEncounter;
    private float waitBeforeShoot_Aiming;
    private float waitAfterShoot;
    private int totalGunmanCount;

    private GameObject bullet;
    private Vector3 myShipPosition;
    private GameObject scaleFactorGameObject;
    private GameObject shipCenter;
    private GameObject gunmanParentObject;
    private readonly GameObject[] gunmen = new GameObject[SetParameters.mediumShipMenCount];
    private readonly GunmanController[] gunmanControllerScript = new GunmanController[SetParameters.mediumShipMenCount];
    private readonly AnimationGunman[] gunmanAnimationScript = new AnimationGunman[SetParameters.mediumShipMenCount];

    private Vector3 endPosition;

    private ShipCategorizer_Level shipCategorizer_LevelScript;
    private ShipCategorizer_Player shipCategorizer_PlayerScript;

    [SerializeField] private int totalAmmoCount;
    private bool sufficientAmmoPresent;

    public Transform targetEnemy;

    private void Awake()
    {
        shipCategorizer_LevelScript = GetComponent<ShipCategorizer_Level>();
        shipCategorizer_PlayerScript = GetComponent<ShipCategorizer_Player>();

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gameObject = transform.GetChild(i).gameObject;
            if (gameObject.name == "ScaleFactorGameObject")
            {
                scaleFactorGameObject = gameObject;
            }
            else if (gameObject.name == "ShipCenter")
            {
                shipCenter = gameObject;
            }
        }

        for (int i = 0; i < scaleFactorGameObject.transform.childCount; i++)
        {
            GameObject gameObject = scaleFactorGameObject.transform.GetChild(i).gameObject;
            if (gameObject.name == "Gunmen")
            {
                gunmanParentObject = gameObject;
            }
        }
        for (int i = 0; i < SetParameters.mediumShipMenCount; i++)
        {
            gunmen[i] = gunmanParentObject.transform.GetChild(i).gameObject;
            gunmanControllerScript[i] = gunmen[i].GetComponent<GunmanController>();
            gunmanAnimationScript[i] = gunmen[i].GetComponent<AnimationGunman>();
        }

        totalGunmanCount = SetParameters.mediumShipMenCount;
        lineWidth = SetParameters.gunmanLineWidth;
        bulletVelocity = SetParameters.gunmanBulletVelocity;
        waitBeforeShoot_FirstEncounter = SetParameters.gunman_WaitBeforeShoot_FirstEncounter;
        sufficientAmmoPresent = true;
    }

    private void Start()
    {
        for (int i = 0; i < totalGunmanCount; i++)
        {
            gunmanControllerScript[i].lineRenderer.startWidth = lineWidth;
            gunmanControllerScript[i].lineRenderer.positionCount = 2;
            gunmanControllerScript[i].enableLineRenderer = true;
        }
        gunmanMaxRange = shipCategorizer_LevelScript.weaponRange;

        if (shipCategorizer_LevelScript.shipLevel == ShipLevels.Level1)
        {
            AssignValue(0);
        }
        else if (shipCategorizer_LevelScript.shipLevel == ShipLevels.Level2)
        {
            AssignValue(1);
        }
        else if (shipCategorizer_LevelScript.shipLevel == ShipLevels.Level3)
        {
            AssignValue(2);
        }
        else if (shipCategorizer_LevelScript.shipLevel == ShipLevels.Level4)
        {
            AssignValue(3);
        }
        targetEnemy = null;
    }

    private void Update()
    {
        myShipPosition = shipCenter.transform.position;
        HandleAmmoCount();

        for (int i = 0; i < totalGunmanCount; i++)
        {
            Transform B = gunmanControllerScript[i].B;
            if (!sufficientAmmoPresent)
            {
                gunmanControllerScript[i].enableLineRenderer = false;//during experimentation, showed linerenderer at previous path points
            }           
            else if (B != null)
            {
                Transform A = gunmanControllerScript[i].A;
                LineRenderer lineRenderer = gunmanControllerScript[i].lineRenderer;
                bool shootOnce = gunmanControllerScript[i].shootOnce;

                bool noEnemyInSight = gunmanControllerScript[i].noEnemyInSight;
                bool shootBullet = gunmanControllerScript[i].shootBullet;

                //float distance = Mathf.Sqrt((B.position.x - shipPosition.x) * (B.position.x - shipPosition.x) + (B.position.y - shipPosition.y) * (B.position.y - shipPosition.y) + (B.position.z - shipPosition.z) * (B.position.z - shipPosition.z));
                float distance = Vector3.Distance(B.position, myShipPosition);

                if (distance < gunmanMaxRange)
                {
                    if (targetEnemy == null)
                    {
                        targetEnemy = B;//Ensure rotation of ship towards enemy
                    }

                    //gunman animation, aiming towards enemy
                    if (lineRenderer.enabled)
                    {
                        gunmanAnimationScript[i].gunmanState = AnimationGunman.GunmanStates.aim;
                    }
                    else
                    {
                        gunmanAnimationScript[i].gunmanState = AnimationGunman.GunmanStates.idle;
                    }

                    lineRenderer.SetPosition(0, Evaluate(0, A, B));//set start point (vertex = 0, position = Evaluate(0))
                    lineRenderer.SetPosition(1, Evaluate(1, A, B));//set end point

                    if (noEnemyInSight)
                    {
                        gunmanControllerScript[i].enableLineRenderer = true;

                        StartCoroutine(WaitForFirstWeaponLoad());
                    }
                    else
                    {
                        //Check if shoot is pressed
                        if (shootBullet)
                        {
                            gunmanControllerScript[i].shootBullet = false;
                            if (!shootOnce)
                            {
                                bullet = objectPoolBulletScript.ReturnProjectile();

                                ProjectileController projectileControllerScript = bullet.GetComponent<ProjectileController>();
                                projectileControllerScript.weaponDamage = shipCategorizer_LevelScript.weaponDamage;
                                projectileControllerScript.isPlayer1Projectile = shipCategorizer_PlayerScript.isP1Ship;

                                //gunman shoot animation
                                gunmanAnimationScript[i].gunmanState = AnimationGunman.GunmanStates.shoot;

                                if (bullet != null)
                                {
                                    bullet.transform.position = A.position;
                                    endPosition = B.transform.position;
                                    gunmanControllerScript[i].shootOnce = true;

                                    gunmanControllerScript[i].enableLineRenderer = false;
                                    StartCoroutine(MoveObject(A.position, endPosition, bullet));
                                    totalAmmoCount--;
                                    StartCoroutine(CoolDownTime());
                                }
                            }
                        }
                    }
                }
                else
                {
                    gunmanControllerScript[i].B = null;//once out of range make sure that the final position is not still pointing to previous ship
                }
            }
            else//B = null
            {
                gunmanAnimationScript[i].gunmanState = AnimationGunman.GunmanStates.idle;

                gunmanControllerScript[i].noEnemyInSight = true;

                if (targetEnemy != null)
                {
                    targetEnemy = null;
                }
            }
        }
    }
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, GameObject bullet)
    {
        bullet.transform.LookAt(endPos);

        float startTime = Time.fixedTime; // used Time.fixedTime instead of just Time.time for better control of arrow velocity
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / bulletVelocity;

        while (Time.fixedTime - startTime < duration)
        {
            float journeyFraction = (Time.fixedTime - startTime) / duration;
            bullet.transform.position = Vector3.Lerp(startPos, endPos, journeyFraction);
            yield return new WaitForFixedUpdate();//used instead of just yield return null
        }

        // Ensure the bullet reaches the exact end position.
        bullet.transform.position = endPos;
    }
    private IEnumerator CoolDownTime()
    {
        yield return new WaitForSeconds(waitBeforeShoot_Aiming);
        for (int i = 0; i < totalGunmanCount; i++)
        {
            gunmanControllerScript[i].enableLineRenderer = true;//display projectile path again
        }

        yield return new WaitForSeconds(waitAfterShoot);
        for (int i = 0; i < totalGunmanCount; i++)
        {
            gunmanControllerScript[i].shootOnce = false;//don't allow shoot to occur even if S is pressed
            gunmanControllerScript[i].shootBullet = true;//display projectile path again
        }
    }
    private IEnumerator WaitForFirstWeaponLoad()
    {
        yield return new WaitForSeconds(waitBeforeShoot_FirstEncounter);
        for (int i = 0; i < totalGunmanCount; i++)
        {
            gunmanControllerScript[i].noEnemyInSight = false;
        }
    }
    private Vector3 Evaluate(float t, Transform A, Transform B)
    {
        Vector3 ab = Vector3.Lerp(A.position, B.position, t);//Interpolate from point A to B
        return ab;
    }
    private void AssignValue(int index)
    {
        waitBeforeShoot_Aiming = SetParameters.gunman_WaitBeforeShoot_Aiming[index];
        waitAfterShoot = SetParameters.gunman_WaitAfterShoot[index];
        totalAmmoCount = SetParameters.gunmanWeaponMaxAmmo[index];
    }
    private void HandleAmmoCount()
    {
        if (totalAmmoCount <= 0)
        {
            sufficientAmmoPresent = false;
        }
        else
        {
            sufficientAmmoPresent = true;
        }
    }
}
