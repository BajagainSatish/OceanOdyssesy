using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    [HideInInspector] public Transform A;
    public Transform B;
    [HideInInspector] public Transform control;
    [HideInInspector] public LineRenderer lineRenderer;
    [HideInInspector] public bool enableLineRenderer;

    [HideInInspector] public bool shootOnce = false;
    [HideInInspector] public Vector3 endPosition;
    [HideInInspector] public Vector3[] routePoints = new Vector3[ArcherShoot.curvePointsTotalCount + 1];
    [HideInInspector] public bool withinArcherRotateRange = false;

    private GameObject projectilePath;
    private GameObject mixamorigHips;

    private readonly int curvePointsTotalCount = ArcherShoot.curvePointsTotalCount;

    private void Awake()
    {
        //Assigning gameobjects in scene to respective fields

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gameObject = transform.GetChild(i).gameObject;
                if (gameObject.name == "ProjectilePath")
                {
                    projectilePath = gameObject;
                    lineRenderer = projectilePath.GetComponent<LineRenderer>();
                }
                else if (gameObject.name == "mixamorig:Hips")
                {
                    mixamorigHips = gameObject;
                }
        }
        for (int k = 0; k < projectilePath.transform.childCount; k++)
        {
            GameObject gameObject = projectilePath.transform.GetChild(k).gameObject;
            if (gameObject.name == "MidControl")
            {
                control = gameObject.transform;
            }
        }
        for (int k = 0; k < mixamorigHips.transform.childCount; k++)
        {
            GameObject gameObject = mixamorigHips.transform.GetChild(k).gameObject;
            if (gameObject.name == "StartPoint")
            {
                A = gameObject.transform;
            }
        }
    }

    private void Update()
    {
        if (B != null)
        {
            if (enableLineRenderer)
            {
                lineRenderer.enabled = true;//show projectile pathway if target is in range
            }
            else
            {
                lineRenderer.enabled = false;//disable line renderer temporarily, eg. during cooldown time
            }
            transform.LookAt(B);//archer faces the target ship
        }
        else
        {
            lineRenderer.enabled = false;//no target, no trajectory
        }
    }

    //Only during scene view, draw lines between intermediate points
    private void OnDrawGizmos()//Draw Quadratic Curve
    {
        if (A == null || B == null || control == null)
        {
            return;
        }
        for (int j = 0; j < curvePointsTotalCount + 1; j++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Evaluate(j / (float)curvePointsTotalCount), Evaluate((j + 1) / (float)curvePointsTotalCount));
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
