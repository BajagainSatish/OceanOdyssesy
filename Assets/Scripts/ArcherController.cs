using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    [HideInInspector] public Transform A;
    public Transform B;
    [HideInInspector] public Transform control;
    [HideInInspector] public LineRenderer lineRenderer;

    [HideInInspector] public bool shootOnce = false;
    [HideInInspector] public Vector3 endPosition;
    [HideInInspector] public Vector3[] routePoints = new Vector3[ArrowShoot.curvePointsTotalCount + 1];
    [HideInInspector] public bool withinArcherRotateRange = false;

    private GameObject projectilePath;
    private int curvePointsTotalCount = ArrowShoot.curvePointsTotalCount;
    private void Awake()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject gameObject = this.transform.GetChild(i).gameObject;
                if (gameObject.name == "ProjectilePath")
                {
                    projectilePath = gameObject;
                    lineRenderer = projectilePath.GetComponent<LineRenderer>();
                }
        }
        for (int k = 0; k < projectilePath.transform.childCount; k++)
        {
            GameObject gameObject = projectilePath.transform.GetChild(k).gameObject;
            if (gameObject.name == "StartPoint")
            {
                A = gameObject.transform;
            }
            else if (gameObject.name == "MidControl")
            {
                control = gameObject.transform;
            }
        }         
    }

    private void Update()
    {
        if (B != null)
        {
            lineRenderer.enabled = true;
            transform.LookAt(B);//archer faces the ship
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void OnDrawGizmos()//Draw Quadratic Curve
    {
        if (A == null || B == null || control == null)
        {
            return;
        }
        for (int j = 0; j < curvePointsTotalCount + 1; j++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Evaluate(j / (float)curvePointsTotalCount), Evaluate((j + 1) / (float)curvePointsTotalCount));//During scene view, draw lines between intermediate points
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(Evaluate(j / (float)curvePointsTotalCount), 0.01f);
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Evaluate(1f), 0.01f);
    }

    private Vector3 Evaluate(float t)//Bezier Curve functionality
    {
        Vector3 ac = Vector3.Lerp(A.position, control.position, t);//Interpolate from point A to ControlPoint
        Vector3 cb = Vector3.Lerp(control.position, B.position, t);//Interpolate from ControlPoint to Point B

        return Vector3.Lerp(ac, cb, t);
    }
}
