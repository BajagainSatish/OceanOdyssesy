using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowShoot : MonoBehaviour
{
    [SerializeField] private ObjectPool_Arrows objectPoolArrowScript;
    [SerializeField] private float lineWidth;
    [SerializeField] private float arrowVelocity;
    [SerializeField] private float leastDistanceForStraightHit;
    [SerializeField] private float adjustCurveAngle;
    [SerializeField] private float archerMaxRange;
    [SerializeField] private float upperYLimit = 0.7f;
    [SerializeField] private float lowerYLimit = -0.1f;
    [SerializeField] private float archerShootAngleRange = 80;

    public static int curvePointsTotalCount = 20;//change this value to change the number of points in curve, and control smoothness of curve by increasing the number
    private static int totalArcherCount = 4;

    private GameObject arrow;
    private Vector3 shipPosition;
    private GameObject scaleFactorGameObject;
    private GameObject shipCenter;
    private GameObject pirates;
    private GameObject[] archers = new GameObject[totalArcherCount];
    private ArcherController[] archerControllerScript = new ArcherController[totalArcherCount];

    private void Awake()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            GameObject gameObject = this.transform.GetChild(i).gameObject;
            if (gameObject.name == "ScaleFactorGameObject")
            {
                scaleFactorGameObject = gameObject;
            }
            else if (gameObject.name == "ShipCenter")
            {
                shipCenter = gameObject;
            }
        }
        for (int i = 0; i < scaleFactorGameObject.transform.childCount; i++)
        {
            GameObject gameObject = scaleFactorGameObject.transform.GetChild(i).gameObject;
            if (gameObject.name == "Pirates")
            {
                pirates = gameObject;
            }
        }
        for (int i = 0; i < totalArcherCount; i++)
        {
            archers[i] = pirates.transform.GetChild(i).gameObject;
            archerControllerScript[i] = archers[i].GetComponent<ArcherController>();
        }
    }

    private void Start()
    {
        for (int i = 0; i < totalArcherCount; i++)
        {
            archerControllerScript[i].lineRenderer.startWidth = lineWidth;
            archerControllerScript[i].lineRenderer.positionCount = curvePointsTotalCount + 1;
        }
    }
    private void Update()
    {
        shipPosition = shipCenter.transform.position;
        for (int i = 0; i < totalArcherCount; i++)
        {
            Transform A = archerControllerScript[i].A;
            Transform B = archerControllerScript[i].B;
            Transform control = archerControllerScript[i].control;

            bool withinArcherRotateRange = archerControllerScript[i].withinArcherRotateRange;
            LineRenderer lineRenderer = archerControllerScript[i].lineRenderer;
            float adjustDistanceFactor = archerControllerScript[i].adjustDistanceFactor;
            bool shootOnce = archerControllerScript[i].shootOnce;
            //GameObject arrow = archerControllerScript[i].arrow;
            Vector3 endPosition = archerControllerScript[i].endPosition;
            Vector3[] routePoints = archerControllerScript[i].routePoints;

            float distance = Mathf.Sqrt((B.position.x - shipPosition.x) * (B.position.x - shipPosition.x) + (B.position.y - shipPosition.y) * (B.position.y - shipPosition.y) + (B.position.z - shipPosition.z) * (B.position.z - shipPosition.z));
            if (Input.GetKeyDown(KeyCode.D))
            {
                print("Distance: " + distance);
            }

            Vector3 difference = B.position - A.position;
            if (Input.GetKey(KeyCode.Space))
            {
                print("Resultant: " + difference);
            }

            Vector3 targetDirection = (B.position - A.position).normalized;
            Vector3 archersForwardDirection = archerControllerScript[i].transform.forward;

            // Calculate the angle between the forward direction and the target direction
            float angle = Vector3.Angle(targetDirection, archersForwardDirection);

            // Check if the angle is within the desired range
            if (angle <= archerShootAngleRange && difference.y >= lowerYLimit && difference.y <= upperYLimit)
            {
                withinArcherRotateRange = true;
            }
            else
            {
                withinArcherRotateRange = false;
            }

            if (distance <= archerMaxRange && withinArcherRotateRange)
            {
                //Draw Line or Curve from archer to enemy
                lineRenderer.enabled = true;
                for (int j = 0; j < curvePointsTotalCount + 1; j++)
                {
                    lineRenderer.SetPosition(j, Evaluate(j / (float)curvePointsTotalCount, A, B, control));
                }

                //Evaluation of straight or curved path
                if (distance < leastDistanceForStraightHit)
                {
                    adjustDistanceFactor = -distance;//experimentally found out, if adjustcurveangle = -distance, straight path
                }
                else
                {
                    adjustDistanceFactor = -(adjustCurveAngle * distance);//curve path for enemy far away
                }

                //Evaluate position for control point on basis of start point and end point
                float p = (A.position.x + B.position.x) / 2f;
                float r = (A.position.z + B.position.z) / 2f;
                float q = (distance + (A.position.y + B.position.y) / 2f) + adjustDistanceFactor;
                control.transform.position = new Vector3(p, q, r);

                //Check if shoot is pressed
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (!shootOnce)
                    {
                        arrow = objectPoolArrowScript.ReturnArrow();
                        print("Arrow " + arrow);
                        print("Arrow " + arrow.name);

                        if (arrow != null)
                        {
                            print("Arrow::::");
                            arrow.transform.position = A.position;
                            endPosition = B.transform.position;
                            for (int j = 0; j < curvePointsTotalCount + 1; j++)
                            {
                                routePoints[j] = Evaluate(j / (float)curvePointsTotalCount, A, B, control);
                            }
                            shootOnce = true;
                            StartCoroutine(MoveThroughRoute(arrow, routePoints));
                        }
                    }
                }
                if (shootOnce)
                {
                    if ((Mathf.Approximately(arrow.transform.position.x, endPosition.x)) && (Mathf.Approximately(arrow.transform.position.y, endPosition.y)) && (Mathf.Approximately(arrow.transform.position.z, endPosition.z)))
                    {
                        print("Arrow movement complete");
                        shootOnce = false;
                    }
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }

    private IEnumerator MoveThroughRoute(GameObject arrow, Vector3[] routePoints)
    {
        for (int i = 0; i < curvePointsTotalCount + 1; i++)
        {
            arrow.transform.LookAt(routePoints[i]);
            yield return StartCoroutine(MoveObject(arrow.transform.position, routePoints[i],arrow));
        }
    }
    private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, GameObject arrow)
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

    private Vector3 Evaluate(float t, Transform A, Transform B, Transform control)//Quadratic Curve functionality
    {
        Vector3 ac = Vector3.Lerp(A.position,control.position,t);//Interpolate from point A to ControlPoint
        Vector3 cb = Vector3.Lerp(control.position, B.position, t);//Interpolate from ControlPoint to Point B

        return Vector3.Lerp(ac,cb,t);
    }
}
