using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowShoot : MonoBehaviour
{
    [SerializeField] private Transform A;
    [SerializeField] private Transform B;
    [SerializeField] private Transform control;
    [SerializeField] private GameObject arrow;
    [SerializeField] private float lineWidth;
    [SerializeField] private float arrowVelocity;
    [SerializeField] private float leastDistanceForStraightHit;
    [SerializeField] private float adjustCurveAngle;

    private static int curvePointsTotalCount = 20;//change this value to change the number of points in curve, and control smoothness of curve by increasing the number
    private float adjustDistanceFactor;
    private LineRenderer lineRenderer;
    private bool shootOnce;
    private Vector3 endPosition;
    private Vector3[] routePoints = new Vector3[curvePointsTotalCount + 1];

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.positionCount = curvePointsTotalCount + 1;
        arrow.transform.position = routePoints[0];
        arrow.transform.LookAt(routePoints[1]);
        shootOnce = false;
    }
    private void Update()
    {
        for (int i = 0; i < curvePointsTotalCount + 1; i++)//1 more for last line to destination point
        {
            lineRenderer.SetPosition(i, Evaluate(i / (float)curvePointsTotalCount));
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

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!shootOnce)
            {
                arrow.transform.position = A.position;
                endPosition = B.transform.position;
                for (int i = 0; i < curvePointsTotalCount + 1; i++)
                {
                    routePoints[i] = Evaluate(i / (float)curvePointsTotalCount);
                }
                shootOnce = true;
                StartCoroutine(MoveThroughRoute());
                //above code executes only once inside update so targetPosition won't be updated if trajectory changes, and ball moves towards previous target
                //similarly the coroutine is also called just once
            }
        }
        if (shootOnce)
        {
            if ((Mathf.Approximately(arrow.transform.position.x,endPosition.x)) && (Mathf.Approximately(arrow.transform.position.y, endPosition.y)) && (Mathf.Approximately(arrow.transform.position.z, endPosition.z)))
            {
                print("Arrow movement complete");
                shootOnce = false;
            }
        }
    }

    private IEnumerator MoveThroughRoute()
    {
        for (int i = 0; i < curvePointsTotalCount + 1; i++)
        {
            arrow.transform.LookAt(routePoints[i]);
            yield return StartCoroutine(MoveObject(arrow.transform.position, routePoints[i]));
        }
    }
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos)
    {
        float startTime = Time.fixedTime; // used Time.fixedTime instead of just Time.time for better control of arrow velocity
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / arrowVelocity;

        while (Time.fixedTime - startTime < duration)
        {
            float journeyFraction = (Time.fixedTime - startTime) / duration;
            arrow.transform.position = Vector3.Lerp(startPos, endPos, journeyFraction);
            yield return new WaitForFixedUpdate();//used instead of just yield return null
        }

        // Ensure the arrow reaches the exact end position.
        arrow.transform.position = endPos;
    }
    private Vector3 Evaluate(float t)//Bezier Curve functionality
    {
        Vector3 ac = Vector3.Lerp(A.position,control.position,t);//Interpolate from point A to ControlPoint
        Vector3 cb = Vector3.Lerp(control.position, B.position, t);//Interpolate from ControlPoint to Point B

        return Vector3.Lerp(ac,cb,t);
    }

    private void OnDrawGizmos()//Draw Quadratic Curve
    {
        if (A==null || B == null || control == null)
        {
            return;
        }
        for (int i = 0; i < curvePointsTotalCount + 1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Evaluate(i/(float)curvePointsTotalCount),Evaluate((i+1)/(float)curvePointsTotalCount));//During scene view, draw lines between intermediate points
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Evaluate(i/(float)curvePointsTotalCount),0.01f);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Evaluate(1f),0.01f);
    }
}
