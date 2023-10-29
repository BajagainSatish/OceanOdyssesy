using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonController : MonoBehaviour
{
    public static int totalCannonCount = 4;

    [SerializeField] private Transform A;
    public Transform B;
    [SerializeField] private float lineWidth;
    [SerializeField] private float cannonBallVelocity;
    [SerializeField] private float coolDownTime;
    public float cannonMaxRange;

    [SerializeField] private GameObject cannonRotator;
    [SerializeField] private GameObject newCannon;

    public ObjectPool_Projectile objectPool_CanonBallScript;
    private ParticleSystem[] smokeParticleEffect = new ParticleSystem[3];
    public static int totalArtilleristCount = 6;//common mortarmen and cannonmen

    [SerializeField] private float cannonShootAngleRange = 60;

    private Vector3 myShipPosition;
    private GameObject cannonBall;
    private LineRenderer lineRenderer;
    private bool shootOnce;
    private Vector3 endPosition;
    private bool withinCannonRotateRange;

    private void Awake()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).name == "SmokeParticleEffects")
            {
                GameObject particleEffectsObject = this.transform.GetChild(i).gameObject;
                for (int j = 0; j < 3; j++)
                {
                    smokeParticleEffect[j] = particleEffectsObject.transform.GetChild(j).GetComponent<ParticleSystem>();
                }
            }
        }       
    }

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.positionCount = 2;
        shootOnce = false;
    }

    private void Update()
    {
        myShipPosition = this.transform.position;//same world position as parent ship

        if (B != null)
        {
            float distance = Vector3.Distance(myShipPosition, B.position);
            if (distance < cannonMaxRange)
            {
                lineRenderer.SetPosition(0, Evaluate(0));//set start point (vertex = 0, position = Evaluate(0))
                lineRenderer.SetPosition(1, Evaluate(1));//set end point

                Vector3 targetDirection = (B.position - newCannon.transform.position).normalized;
                Vector3 cannonsForwardDirection = newCannon.transform.forward;//will remain constant

                // Calculate the angle between the forward direction and the target direction
                float angle = Vector3.Angle(targetDirection, cannonsForwardDirection);

                // Check if the angle is within the desired range
                if (angle < cannonShootAngleRange)
                {
                    withinCannonRotateRange = true;
                }
                else
                {
                    withinCannonRotateRange = false;
                }

                if (withinCannonRotateRange)
                {
                    lineRenderer.enabled = true;
                    lineRenderer.SetPosition(0, Evaluate(0, A, B));//set start point (vertex = 0, position = Evaluate(0))
                    lineRenderer.SetPosition(1, Evaluate(1, A, B));//set end point
                    cannonRotator.transform.LookAt(B.position);//we set the x-rotation of gameobject to -8, so that the gameobject aligns with the cannons shooting end

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        if (!shootOnce)
                        {
                            cannonBall = objectPool_CanonBallScript.ReturnProjectile();

                            if (cannonBall != null)
                            {
                                cannonBall.transform.position = A.position;
                                endPosition = B.transform.position;
                                shootOnce = true;
                                StartCoroutine(MoveObject(A.position, endPosition, cannonBall));
                                StartCoroutine(CoolDownTime());
                            }

                            //above code executes only once inside update so targetPosition won't be updated if trajectory changes, and bullet moves towards previous target
                            //similarly the coroutine is also called just once
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
                lineRenderer.enabled = false;//persisting line renderer is no longer visible
            }
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
        yield return new WaitForSeconds(coolDownTime);
        for (int i = 0; i < totalArtilleristCount; i++)
        {
            shootOnce = false;
        }
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
}
