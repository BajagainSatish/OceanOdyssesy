using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [SerializeField] private ObjectPool_Projectile objectPoolBulletScript;
    [SerializeField] private float lineWidth;
    [SerializeField] private float bulletVelocity;
    public float gunmanMaxRange;
    [SerializeField] private float coolDownTime;

    public static int totalGunmanCount = 8;

    private GameObject bullet;
    private Vector3 myShipPosition;
    private GameObject scaleFactorGameObject;
    private GameObject shipCenter;
    private GameObject gunmanParentObject;
    private readonly GameObject[] gunmen = new GameObject[totalGunmanCount];
    private readonly GunmanController[] gunmanControllerScript = new GunmanController[totalGunmanCount];
    private readonly AnimationGunman[] gunmanAnimationScript = new AnimationGunman[totalGunmanCount];

    private ShipClassifier shipClassifierScript;

    private Vector3 endPosition;
    private bool shipIsActive;
    private bool hasNotShotEvenOnce;//ensure that line renderer is visible at start if enemy ship is inside range, once visible it has no other significance

    private void Awake()
    {
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
        for (int i = 0; i < totalGunmanCount; i++)
        {
            gunmen[i] = gunmanParentObject.transform.GetChild(i).gameObject;
            gunmanControllerScript[i] = gunmen[i].GetComponent<GunmanController>();
            gunmanAnimationScript[i] = gunmen[i].GetComponent<AnimationGunman>();
        }
        shipClassifierScript = GetComponent<ShipClassifier>();
    }

    private void Start()
    {
        for (int i = 0; i < totalGunmanCount; i++)
        {
            gunmanControllerScript[i].lineRenderer.startWidth = lineWidth;
            gunmanControllerScript[i].lineRenderer.positionCount = 2;
            gunmanControllerScript[i].enableLineRenderer = false;
        }
        hasNotShotEvenOnce = true;
        shipIsActive = false;
    }

    private void Update()
    {
        shipIsActive = shipClassifierScript.isActive;
        myShipPosition = shipCenter.transform.position;

        for (int i = 0; i < totalGunmanCount; i++)
        {
            Transform B = gunmanControllerScript[i].B;

            if (B != null)
            {
                Transform A = gunmanControllerScript[i].A;
                LineRenderer lineRenderer = gunmanControllerScript[i].lineRenderer;
                bool shootOnce = gunmanControllerScript[i].shootOnce;

                //float distance = Mathf.Sqrt((B.position.x - shipPosition.x) * (B.position.x - shipPosition.x) + (B.position.y - shipPosition.y) * (B.position.y - shipPosition.y) + (B.position.z - shipPosition.z) * (B.position.z - shipPosition.z));
                float distance = Vector3.Distance(B.position, myShipPosition);

                //Show line renderer only for currently selected ship, and if not during cool down time
                if (shipIsActive && gunmanControllerScript[i].enableLineRenderer)
                {
                    gunmanControllerScript[i].enableLineRenderer = true;
                }
                else if (!shipIsActive)
                {
                    gunmanControllerScript[i].enableLineRenderer = false;
                }

                if (distance < gunmanMaxRange)
                {
                    if (hasNotShotEvenOnce && shipIsActive)
                    {
                        gunmanControllerScript[i].enableLineRenderer = true;
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

                    //Check if shoot is pressed
                    if (Input.GetKeyDown(KeyCode.S) && shipIsActive)//shoot only if ship is selected
                    {
                        if (hasNotShotEvenOnce)
                        {
                            hasNotShotEvenOnce = false;
                        }
                        if (!shootOnce)
                        {
                            bullet = objectPoolBulletScript.ReturnProjectile();

                            //gunman shoot animation
                            gunmanAnimationScript[i].gunmanState = AnimationGunman.GunmanStates.shoot;

                            if (bullet != null)
                            {
                                bullet.transform.position = A.position;
                                endPosition = B.transform.position;

                                gunmanControllerScript[i].shootOnce = true;
                                StartCoroutine(MoveObject(A.position, endPosition, bullet));
                                gunmanControllerScript[i].enableLineRenderer = false;
                                StartCoroutine(CoolDownTime());
                            }
                        }
                    }
                }
                else
                {
                    gunmanControllerScript[i].B = null;//once out of range make sure that the final position is not still pointing to previous ship
                }
            }
            else
            {
                gunmanAnimationScript[i].gunmanState = AnimationGunman.GunmanStates.idle;
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
        yield return new WaitForSeconds(coolDownTime);
        for (int i = 0; i < totalGunmanCount; i++)
        {
            gunmanControllerScript[i].shootOnce = false;
            gunmanControllerScript[i].enableLineRenderer = true;
        }
    }

    private Vector3 Evaluate(float t, Transform A, Transform B)
    {
        Vector3 ab = Vector3.Lerp(A.position, B.position, t);//Interpolate from point A to B
        return ab;
    }
}
