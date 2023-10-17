using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public Transform control;
    public LineRenderer lineRenderer;

    public float adjustDistanceFactor;
    public bool shootOnce = false;
    public Vector3 endPosition;
    public Vector3[] routePoints = new Vector3[ArrowShoot.curvePointsTotalCount + 1];
    public bool withinArcherRotateRange = false;

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
    /*
             Vector3 difference = B.position - A.position;
        float x = difference.x;
        float z = difference.z;

        if (this.transform.eulerAngles.y == 0)
        {
            if (z >= 0)
            {
                withinArcherRotateRange = true;
            }
            else
            {
                withinArcherRotateRange = false;
            }
        }
        else if (this.transform.eulerAngles.y == 180)
        {
            if (z <= 0)
            {
                withinArcherRotateRange = true;
            }
            else
            {
                withinArcherRotateRange = false;
            }
        }
        else if (this.transform.eulerAngles.y == 90)
        {
            if (x >= 0)
            {
                withinArcherRotateRange = true;
            }
            else
            {
                withinArcherRotateRange = false;
            }
        }
        else if (this.transform.eulerAngles.y == 270)
        {
            if (x <= 0)
            {
                withinArcherRotateRange = true;
            }
            else
            {
                withinArcherRotateRange = false;
            }
        }
        else if (Mathf.Approximately(this.transform.eulerAngles.y, 45.0f))
        {
            if ((x > 0 && z > 0) || (x < 0 && z > 0 && z > Mathf.Abs(x)) || (x > 0 && z < 0 && x > Mathf.Abs(z)))
            {
                withinArcherRotateRange = true;
            }
            else
            {
                withinArcherRotateRange = false;
            }
        }
        else if (Mathf.Approximately(this.transform.eulerAngles.y, 225.0f))
        {
            if ((x < 0 && z < 0) || (x < 0 && z > 0 && z < Mathf.Abs(x)) || (x > 0 && z < 0 && x < Mathf.Abs(z)))
            {
                withinArcherRotateRange = true;
            }
            else
            {
                withinArcherRotateRange = false;
            }
        }
        else if (Mathf.Approximately(this.transform.eulerAngles.y, 315.0f))
        {
            if ((x < 0 && z > 0) || (x < 0 && z < 0 && Mathf.Abs(x) > Mathf.Abs(z)) || (x > 0 && z > 0 && x < z))
            {
                withinArcherRotateRange = true;
            }
            else
            {
                withinArcherRotateRange = false;
            }
        }
        else if (Mathf.Approximately(this.transform.eulerAngles.y, 135.0f))
        {
            if ((x > 0 && z < 0) || (x < 0 && z < 0 && Mathf.Abs(z) > Mathf.Abs(x)) || (x > 0 && z > 0 && x > z))
            {
                withinArcherRotateRange = true;
            }
            else
            {
                withinArcherRotateRange = false;
            }
        }
    */
}
