using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [SerializeField] private Transform A;
    [SerializeField] private Transform B;
    [SerializeField] private GameObject bullet;
    [SerializeField] private float lineWidth;
    [SerializeField] private float bulletVelocity;

    private LineRenderer lineRenderer;
    private bool shootOnce;
    private Vector3 endPosition;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.positionCount = 2;
        bullet.transform.position = A.position;
        bullet.transform.LookAt(B.position);
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
                bullet.transform.position = A.position;
                endPosition = B.transform.position;

                shootOnce = true;
                StartCoroutine(MoveObject(A.position,endPosition));
                //above code executes only once inside update so targetPosition won't be updated if trajectory changes, and bullet moves towards previous target
                //similarly the coroutine is also called just once
            }
        }
        if (shootOnce)
        {
            if ((Mathf.Approximately(bullet.transform.position.x, endPosition.x)) && (Mathf.Approximately(bullet.transform.position.y, endPosition.y)) && (Mathf.Approximately(bullet.transform.position.z, endPosition.z)))
            {
                print("Bullet movement complete");
                shootOnce = false;
            }
        }
    }
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos)
    {
        bullet.transform.LookAt(endPos);

        float startTime = Time.fixedTime; // used Time.fixedTime instead of just Time.time for better control of arrow velocity
        float distance = Vector3.Distance(startPos, endPos);
        float duration = distance / bulletVelocity;

        while (Time.fixedTime - startTime < duration)
        {
            float journeyFraction = (Time.fixedTime - startTime) / duration;
            bullet.transform.position = Vector3.Lerp(startPos, endPos, journeyFraction);
            yield return new WaitForFixedUpdate();//used instead of just yield return null
        }

        // Ensure the bullet reaches the exact end position.
        bullet.transform.position = endPos;
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

}
