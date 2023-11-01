using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunmanController : MonoBehaviour
{
    public Transform A;
    public Transform B;
    public LineRenderer lineRenderer;

    public bool shootOnce = false;
    public bool enableLineRenderer;

    private GameObject projectilePath;
    private GameObject mixamorigHips;
    public GameObject rifle;

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
            else if (gameObject.name == "mixamorig:Hips")
            {
                mixamorigHips = gameObject;
            }
        }
        for (int i = 0; i < mixamorigHips.transform.childCount; i++)
        {
            GameObject gameObject = mixamorigHips.transform.GetChild(i).gameObject;
            if (gameObject.name == "rifle")
            {
                rifle = gameObject;
            }
        }
        for (int i = 0; i < rifle.transform.childCount; i++)
        {
            GameObject gameObject = rifle.transform.GetChild(i).gameObject;
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
            transform.LookAt(B);//archer faces the ship
            if (enableLineRenderer)
            {
                lineRenderer.enabled = true;
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
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

    private Vector3 Evaluate(float t)
    {
        Vector3 ab = Vector3.Lerp(A.position, B.position, t);//Interpolate from point A to B
        return ab;
    }
}
