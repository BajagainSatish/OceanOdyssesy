using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortarShoot : MonoBehaviour
{
    [SerializeField] private Transform A;
    [SerializeField] private Transform B;
    [SerializeField] private Transform control;
    [SerializeField] private ObjectPool_Projectile objectPoolMortarScript;

    [SerializeField] private float lineWidth;
    [SerializeField] private float mortarBombVelocity;
    [SerializeField] private float adjustCurveAngle;
    [SerializeField] private float coolDownTime;

    private static int curvePointsTotalCount = 20;//change this value to change the number of points in curve, and control smoothness of curve by increasing the number
    public static int totalArtilleristCount = 6;//common mortarmen and cannonmen

    private GameObject mortarBomb;
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
        shootOnce = false;
    }

    private void Update()
    {
        for (int i = 0; i < curvePointsTotalCount + 1; i++)//1 more for last line to destination point
        {
            lineRenderer.SetPosition(i, Evaluate(i / (float)curvePointsTotalCount));
        }
        //Evaluate proper position for middle control point of curve
        float distance = Mathf.Sqrt((B.position.x - A.position.x) * (B.position.x - A.position.x) + (B.position.y - A.position.y) * (B.position.y - A.position.y) + (B.position.z - A.position.z) * (B.position.z - A.position.z));

        if (Input.GetKeyDown(KeyCode.D))
        {
            print("Distance: " + distance);
        }

        adjustDistanceFactor = -(adjustCurveAngle * distance);

        float p = (A.position.x + B.position.x) / 2f;
        float r = (A.position.z + B.position.z) / 2f;
        float q = (distance + (A.position.y + B.position.y) / 2f) + adjustDistanceFactor;

        control.transform.position = new Vector3(p, q, r);

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!shootOnce)
            {
                mortarBomb = objectPoolMortarScript.ReturnProjectile();

                if (mortarBomb != null)
                {
                    mortarBomb.transform.position = A.position;
                    endPosition = B.transform.position;
                    for (int i = 0; i < curvePointsTotalCount + 1; i++)
                    {
                        routePoints[i] = Evaluate(i / (float)curvePointsTotalCount);
                    }
                    shootOnce = true;
                    StartCoroutine(MoveThroughRoute());
                    StartCoroutine(CoolDownTime());
                }               
                //above code executes only once inside update so targetPosition won't be updated if trajectory changes, and ball moves towards previous target
                //similarly the coroutine is also called just once
            }
        }
        if (shootOnce)
        {
            if ((Mathf.Approximately(mortarBomb.transform.position.x, endPosition.x)) && (Mathf.Approximately(mortarBomb.transform.position.y, endPosition.y)) && (Mathf.Approximately(mortarBomb.transform.position.z, endPosition.z)))
            {
                print("Mortarbomb movement complete");
                shootOnce = false;
            }
        }
    }

    private IEnumerator MoveThroughRoute()
    {
        for (int i = 0; i < curvePointsTotalCount + 1; i++)
        {
            mortarBomb.transform.LookAt(routePoints[i]);
            yield return StartCoroutine(MoveObject(mortarBomb.transform.position, routePoints[i]));
        }
    }
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos)
    {
        float startTime = Time.fixedTime; // used Time.fixedTime instead of just Time.time for better control of arrow velocity
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / mortarBombVelocity;

        while (Time.fixedTime - startTime < duration)
        {
            float journeyFraction = (Time.fixedTime - startTime) / duration;
            mortarBomb.transform.position = Vector3.Lerp(startPos, endPos, journeyFraction);
            yield return new WaitForFixedUpdate();//used instead of just yield return null
        }

        // Ensure the mortar bomb reaches the exact end position.
        mortarBomb.transform.position = endPos;
    }
    private IEnumerator CoolDownTime()
    {
        yield return new WaitForSeconds(coolDownTime);
        for (int i = 0; i < totalArtilleristCount; i++)
        {
            shootOnce = false;
        }
    }

    private Vector3 Evaluate(float t)//Quadratic Curve functionality
    {
        Vector3 ac = Vector3.Lerp(A.position, control.position, t);//Interpolate from point A to ControlPoint
        Vector3 cb = Vector3.Lerp(control.position, B.position, t);//Interpolate from ControlPoint to Point B

        return Vector3.Lerp(ac, cb, t);
    }

    private void OnDrawGizmos()//Draw Quadratic Curve
    {
        if (A == null || B == null || control == null)
        {
            return;
        }
        for (int i = 0; i < curvePointsTotalCount + 1; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Evaluate(i / (float)curvePointsTotalCount), Evaluate((i + 1) / (float)curvePointsTotalCount));//During scene view, draw lines between intermediate points
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Evaluate(i / (float)curvePointsTotalCount), 0.01f);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Evaluate(1f), 0.01f);
    }
}
