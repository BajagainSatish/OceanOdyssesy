using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArcherController : MonoBehaviour
{
    [SerializeField] private Transform A;
    [SerializeField] private Transform B;
    [SerializeField] private Transform control;
    [SerializeField] private GameObject sphere;
    [SerializeField] private float lineWidth;
    [SerializeField] private float desiredDuration;
    [SerializeField] private float leastDistanceForStraightHit;
    [SerializeField] private float adjustCurveAngle;
    [SerializeField] private float coolDownDuration = 0.5f;

    private float adjustDistanceFactor;
    private LineRenderer lineRenderer;
    private int intermediateLinePointsCount;
    private float elapsedTime;
    private bool shootOnce;
    private Vector3 endPosition;
    private Vector3[] routePoints = new Vector3[21];
    private bool shootCoolDown = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(lineWidth,lineWidth);
        intermediateLinePointsCount = 20;
        lineRenderer.positionCount = intermediateLinePointsCount + 1;

        sphere.transform.position = A.transform.position;
        shootOnce = false;
    }
    private void Update()
    {
        for (int i = 0; i < intermediateLinePointsCount + 1; i++)//1 more for last line to destination point
        {
            lineRenderer.SetPosition(i, Evaluate(i / 20f));
        }
        //Evaluate proper position for middle control point of curve
        float distance = Mathf.Sqrt((B.position.x - A.position.x)* (B.position.x - A.position.x) + (B.position.y - A.position.y) * (B.position.y - A.position.y) + (B.position.z - A.position.z) * (B.position.z - A.position.z));

        if (Input.GetKeyDown(KeyCode.D))
        {
            print("Distance: " + distance);
        }

        if (distance < leastDistanceForStraightHit)
        {
            adjustDistanceFactor = -distance;//experimentally found out, if adjustcurveangle = -distance, straight path
        }
        else
        {
            adjustDistanceFactor = -(adjustCurveAngle * distance);
        }

        float p = (A.position.x + B.position.x) / 2f;
        float r = (A.position.z + B.position.z) / 2f;
        float q = (distance + (A.position.y + B.position.y) / 2f) + adjustDistanceFactor;

        control.transform.position = new Vector3(p,q,r);

        if (Input.GetKeyDown(KeyCode.S) && !shootCoolDown)
        {
            shootCoolDown = true;
            StartCoroutine(ShootCooldownTimer());

            if (!shootOnce)
            {
                elapsedTime = 0;
                endPosition = B.transform.position;
                for (int i = 0; i < 21; i++)
                {
                    routePoints[i] = Evaluate(i / 20f);
                }
                shootOnce = true;
                //above code executes only once inside update so targetPosition won't be updated if trajectory changes, and ball moves towards previous target
            }
        }
        if (shootOnce)
        {
            StartCoroutine(MoveThroughRoute());
            MoveObject(sphere.transform.position, endPosition);
            if ((Mathf.Approximately(sphere.transform.position.x,endPosition.x)) && (Mathf.Approximately(sphere.transform.position.y, endPosition.y)) && (Mathf.Approximately(sphere.transform.position.z, endPosition.z)))
            {
                print("position reached");
                shootOnce = false;
            }
        }
    }

    private IEnumerator MoveThroughRoute()
    {
        for (int i = 0; i < 20; i++)
        {
            MoveObject(routePoints[i], routePoints[i+1]);
            yield return null;
        }
    }

    private void MoveObject(Vector3 startPos, Vector3 endPos)
    {
        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / desiredDuration;
        sphere.transform.position = Vector3.Lerp(startPos, endPos, percentageComplete);
    }

    private IEnumerator ShootCooldownTimer()//prevent buggy arrow movement when S pressed simultaneously
    {
        yield return new WaitForSeconds(coolDownDuration);
        shootCoolDown = false;
    }

    public Vector3 Evaluate(float t)//Bezier Curve functionality
    {
        Vector3 ac = Vector3.Lerp(A.position,control.position,t);//Interpolate from point A to ControlPoint
        Vector3 cb = Vector3.Lerp(control.position, B.position, t);//Interpolate from ControlPoint to Point B

        return Vector3.Lerp(ac,cb,t);
    }

    private void OnDrawGizmos()//Draw Bezier Curve
    {
        Gizmos.color = Color.red;
        if (A==null || B == null || control == null)
        {
            return;
        }
        for (int i = 0; i < 20; i++)
        {
            Gizmos.DrawLine(Evaluate(i/20f),Evaluate((i+1)/20f));//During scene and game view, draw lines between intermediate points
        }
    }
}
