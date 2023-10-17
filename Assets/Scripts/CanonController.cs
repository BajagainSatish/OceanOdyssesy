using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonController : MonoBehaviour
{
    [SerializeField] private Transform A;
    [SerializeField] private Transform B;
    [SerializeField] private GameObject Cannonball;
    [SerializeField] private float lineWidth;
    [SerializeField] private float cannonBallVelocity;

    public ObjectPool_CanonBall objectPool_CanonBallScript;
    private ParticleSystem[] smokeParticleEffect = new ParticleSystem[3];

    private LineRenderer lineRenderer;
    private bool shootOnce;
    private Vector3 endPosition;

    private void Awake()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).name == "ObjectPool_CanonBalls")
            {
                objectPool_CanonBallScript = this.transform.GetChild(i).GetComponent<ObjectPool_CanonBall>();
            }
            else if (this.transform.GetChild(i).name == "SmokeParticleEffects")
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
        Cannonball.transform.position = A.position;
        Cannonball.transform.LookAt(B.position);
        shootOnce = false;
    }

    private void Update()
    {
        lineRenderer.SetPosition(0, Evaluate(0));//set start point (vertex = 0, position = Evaluate(0))
        lineRenderer.SetPosition(1, Evaluate(1));//set end point

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!shootOnce)
            {
                Cannonball.transform.position = A.position;
                endPosition = B.transform.position;

                shootOnce = true;
                StartCoroutine(MoveObject(A.position, endPosition));
                //above code executes only once inside update so targetPosition won't be updated if trajectory changes, and bullet moves towards previous target
                //similarly the coroutine is also called just once
            }
        }
        if (shootOnce)
        {
            if ((Mathf.Approximately(Cannonball.transform.position.x, endPosition.x)) && (Mathf.Approximately(Cannonball.transform.position.y, endPosition.y)) && (Mathf.Approximately(Cannonball.transform.position.z, endPosition.z)))
            {
                print("Cannonball movement complete");
                shootOnce = false;
            }
        }
    }
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos)
    {
        Cannonball.transform.LookAt(endPos);

        float startTime = Time.fixedTime; // used Time.fixedTime instead of just Time.time for better control of arrow velocity
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / cannonBallVelocity;

        while (Time.fixedTime - startTime < duration)
        {
            float journeyFraction = (Time.fixedTime - startTime) / duration;
            Cannonball.transform.position = Vector3.Lerp(startPos, endPos, journeyFraction);
            yield return new WaitForFixedUpdate();//used instead of just yield return null
        }

        // Ensure the bullet reaches the exact end position.
        Cannonball.transform.position = endPos;
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
        GameObject newCanonBall = objectPool_CanonBallScript.ReturnCanonBall();
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
}
