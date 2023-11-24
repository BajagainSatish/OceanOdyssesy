using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [SerializeField] private Transform A;
    public Transform B;

    private float lineWidth;
    private float cannonBallVelocity;
    private float cannonMaxRange;
    private float cannonShootAngleRange;
    private float waitBeforeShoot_FirstEncounter;
    private float waitBeforeShoot_Aiming;
    private float waitAfterShoot;

    [SerializeField] private GameObject cannonRotator;
    public GameObject newCannon;

    public ObjectPool_Projectile objectPool_CanonBallScript;
    private readonly ParticleSystem[] smokeParticleEffect = new ParticleSystem[3];

    private Transform shipGameObject;
    private Transform myShipCenter;

    private Vector3 myShipPosition;
    private GameObject cannonBall;
    public LineRenderer lineRenderer;
    private bool shootOnce;
    private Vector3 endPosition;
    private bool withinCannonRotateRange;
    public bool enableLineRenderer;

    private bool shootCannonBall;
    private bool noEnemyInSight;

    private ShipCategorizer_Level shipCategorizer_LevelScript;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "SmokeParticleEffects")
            {
                GameObject particleEffectsObject = transform.GetChild(i).gameObject;
                for (int j = 0; j < 3; j++)
                {
                    smokeParticleEffect[j] = particleEffectsObject.transform.GetChild(j).GetComponent<ParticleSystem>();
                }
            }
        }

        lineWidth = SetParameters.cannonLineWidth;
        cannonBallVelocity = SetParameters.cannonBallVelocity;
        cannonShootAngleRange = SetParameters.cannonShootAngleRange;
        waitBeforeShoot_FirstEncounter = SetParameters.cannon_WaitBeforeShoot_FirstEncounter;
        waitBeforeShoot_Aiming = SetParameters.cannon_WaitBeforeShoot_Aiming;
        waitAfterShoot = SetParameters.cannon_WaitAfterShoot;      
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.positionCount = 2;
        shootOnce = false;
        shipGameObject = MortarController.FindHighestParent(transform);
        myShipCenter = shipGameObject.GetChild(0);
        enableLineRenderer = true;

        shootCannonBall = true;
        noEnemyInSight = true;

        shipCategorizer_LevelScript = shipGameObject.GetComponent<ShipCategorizer_Level>();
        cannonMaxRange = shipCategorizer_LevelScript.weaponRange;

        print(this.name + cannonMaxRange);
    }

    private void Update()
    {
        myShipPosition = myShipCenter.position;

        if (B != null)
        {
            float distance = Vector3.Distance(myShipPosition, B.position);

            if (distance < cannonMaxRange)
            {
                lineRenderer.SetPosition(0, Evaluate(0));//set start point (vertex = 0, position = Evaluate(0))
                lineRenderer.SetPosition(1, Evaluate(1));//set end point

                withinCannonRotateRange = CannonRangeCheck(newCannon,B,cannonShootAngleRange);

                if (withinCannonRotateRange)
                {
                    if (enableLineRenderer)
                    {
                        lineRenderer.enabled = true;
                    }
                    else
                    {
                        lineRenderer.enabled = false;
                    }

                    lineRenderer.SetPosition(0, Evaluate(0, A, B));//set start point (vertex = 0, position = Evaluate(0))
                    lineRenderer.SetPosition(1, Evaluate(1, A, B));//set end point
                    cannonRotator.transform.LookAt(B.position);//we set the x-rotation of gameobject to -8, so that the gameobject aligns with the cannons shooting end

                    if (noEnemyInSight)
                    {
                        enableLineRenderer = true;
                        StartCoroutine(WaitForFirstWeaponLoad());
                    }
                    else
                    {
                        if (shootCannonBall)
                        {
                            shootCannonBall = false;
                            if (!shootOnce)
                            {
                                cannonBall = objectPool_CanonBallScript.ReturnProjectile();

                                if (cannonBall != null)
                                {
                                    cannonBall.transform.position = A.position;
                                    endPosition = B.transform.position;
                                    shootOnce = true;
                                    enableLineRenderer = false;
                                    StartCoroutine(MoveObject(A.position, endPosition, cannonBall));
                                    StartCoroutine(CoolDownTime());
                                }
                                //above code executes only once inside update so targetPosition won't be updated if trajectory changes, and bullet moves towards previous target
                                //similarly the coroutine is also called just once
                            }
                        }
                    }
                }
                else
                {
                    lineRenderer.enabled = false;
                }
            }
            else
            {
                B = null;
            }
        }
        else//B = null
        {
            lineRenderer.enabled = false;//persisting line renderer is no longer visible
            noEnemyInSight = true;
        }
    }
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, GameObject cannonBall)
    {
        cannonBall.transform.LookAt(endPos);
        
        float startTime = Time.fixedTime; // used Time.fixedTime instead of just Time.time for better control of arrow velocity
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / cannonBallVelocity;

        while (Time.fixedTime - startTime < duration)
        {
            float journeyFraction = (Time.fixedTime - startTime) / duration;
            cannonBall.transform.position = Vector3.Lerp(startPos, endPos, journeyFraction);
            yield return new WaitForFixedUpdate();//used instead of just yield return null
        }

        // Ensure the bullet reaches the exact end position.
        cannonBall.transform.position = endPos;
    }
    private IEnumerator CoolDownTime()
    {
        yield return new WaitForSeconds(waitBeforeShoot_Aiming);
        enableLineRenderer = true;
        yield return new WaitForSeconds(waitAfterShoot);
        shootOnce = false;
        shootCannonBall = true;
    }
    private IEnumerator WaitForFirstWeaponLoad()
    {
        yield return new WaitForSeconds(waitBeforeShoot_FirstEncounter);
        noEnemyInSight = false;
    }
    private Vector3 Evaluate(float t)
    {
        Vector3 ab = Vector3.Lerp(A.position, B.position, t);//Interpolate from point A to B
        return ab;
    }

    private void OnDrawGizmos()//Draw Straight line between start and end points
    {
        if (A == null || B == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Evaluate(0), Evaluate(1));//Only during scene view, draw a line between points
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Evaluate(0), 0.01f);
        Gizmos.DrawWireSphere(Evaluate(1), 0.01f);
    }

    public void ShootCanonBall()
    {
        GameObject newCanonBall = objectPool_CanonBallScript.ReturnProjectile();
        if (newCanonBall != null)
        {
            newCanonBall.transform.position = A.position;
            newCanonBall.SetActive(true);
            newCanonBall.GetComponent<Rigidbody>().velocity = A.transform.forward * cannonBallVelocity;
            for (int i = 0; i < 3; i++)
            {
                smokeParticleEffect[i].Play();
            }
            //Initially x-rotation of shootpoint set to -10, tweak it as necessary
        }
    }
    private Vector3 Evaluate(float t, Transform A, Transform B)
    {
        Vector3 ab = Vector3.Lerp(A.position, B.position, t);//Interpolate from point A to B
        return ab;
    }

    public bool CannonRangeCheck(GameObject cannon, Transform targetPos, float shootAngleRange)
    {
        bool withinRotateRange;
        if (targetPos == null)
        {
            print("targetPos null");
            return false;
        }

        Vector3 targetDirection = (targetPos.position - cannon.transform.position).normalized;
        Vector3 cannonsForwardDirection = cannon.transform.forward;//will remain constant

        // Calculate the angle between the forward direction and the target direction
        float angle = Vector3.Angle(targetDirection, cannonsForwardDirection);

        // Check if the angle is within the desired range
        if (angle < shootAngleRange)
        {
            withinRotateRange = true;
        }
        else
        {
            withinRotateRange = false;
        }
        return withinRotateRange;
    }
}
