using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherShoot : MonoBehaviour
{
    [SerializeField] private ObjectPool_Projectile objectPoolArrowScript;
    [SerializeField] private float lineWidth;
    [SerializeField] private float arrowVelocity;
    [SerializeField] private float leastDistanceForStraightHit;
    [SerializeField] private float adjustCurveAngle;
    public float archerMaxRange;
    [SerializeField] private float coolDownTime;

    public static int curvePointsTotalCount = 20;//change this value to change the number of points in curve, and control smoothness of curve by increasing the number
    public static int totalArcherCount = 4;

    private GameObject arrow;
    private Vector3 myShipPosition;
    private GameObject scaleFactorGameObject;
    private GameObject myShipCenter;
    private GameObject archerParentObject;
    private GameObject[] archers = new GameObject[totalArcherCount];
    private ArcherController[] archerControllerScript = new ArcherController[totalArcherCount];
    private AnimationArcher[] archerAnimatorScript = new AnimationArcher[totalArcherCount];

    private bool hasNotShotEvenOnce;//ensure that line renderer is visible at start if enemy ship is inside range, once visible it has no other significance
    private float adjustDistanceFactor;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject gameObject = transform.GetChild(i).gameObject;
            if (gameObject.name == "ScaleFactorGameObject")
            {
                scaleFactorGameObject = gameObject;
            }
            else if (gameObject.name == "ShipCenter")
            {
                myShipCenter = gameObject;
            }
        }
        for (int i = 0; i < scaleFactorGameObject.transform.childCount; i++)
        {
            GameObject gameObject = scaleFactorGameObject.transform.GetChild(i).gameObject;
            if (gameObject.name == "Shooter")
            {
                archerParentObject = gameObject;
            }
        }
        for (int i = 0; i < totalArcherCount; i++)
        {
            archers[i] = archerParentObject.transform.GetChild(i).gameObject;
            archerControllerScript[i] = archers[i].GetComponent<ArcherController>();
            archerAnimatorScript[i] = archers[i].GetComponent<AnimationArcher>();
        }
    }

    private void Start()
    {
        for (int i = 0; i < totalArcherCount; i++)
        {
            archerControllerScript[i].lineRenderer.startWidth = lineWidth;
            archerControllerScript[i].lineRenderer.positionCount = curvePointsTotalCount + 1;
            archerControllerScript[i].enableLineRenderer = false;
        }
        hasNotShotEvenOnce = true;
    }
    private void Update()
    {
        myShipPosition = myShipCenter.transform.position;
        for (int i = 0; i < totalArcherCount; i++)
        {
            Transform B = archerControllerScript[i].B;
            if (B != null)
            {
                Transform A = archerControllerScript[i].A;
                Transform control = archerControllerScript[i].control;

                LineRenderer lineRenderer = archerControllerScript[i].lineRenderer;
                bool shootOnce = archerControllerScript[i].shootOnce;
                Vector3[] routePoints = archerControllerScript[i].routePoints;

                float distance = Vector3.Distance(B.position, myShipPosition);

                if (distance < archerMaxRange)
                {
                    if (hasNotShotEvenOnce)
                    {
                        archerControllerScript[i].enableLineRenderer = true;
                    }

                    //archer animation, aiming towards enemy
                    if (lineRenderer.enabled)
                    {
                        archerAnimatorScript[i].archerState = AnimationArcher.ArcherStates.aim;
                    }
                    else
                    {
                        archerAnimatorScript[i].archerState = AnimationArcher.ArcherStates.idle;

                    }

                    //Draw Curve from archer to enemy
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
                        if (hasNotShotEvenOnce)
                        {
                            hasNotShotEvenOnce = false;
                        }
                        if (!shootOnce)
                        {
                            arrow = objectPoolArrowScript.ReturnProjectile();

                            //archer shoot animation
                            archerAnimatorScript[i].archerState = AnimationArcher.ArcherStates.shoot;

                            if (arrow != null)
                            {
                                arrow.transform.position = A.position;

                                for (int j = 0; j < curvePointsTotalCount + 1; j++)
                                {
                                    routePoints[j] = Evaluate(j / (float)curvePointsTotalCount, A, B, control);
                                }
                                archerControllerScript[i].shootOnce = true;

                                StartCoroutine(MoveThroughRoute(arrow, routePoints));
                                archerControllerScript[i].enableLineRenderer = false;//disable projectile path for cool down time
                                StartCoroutine(CoolDownTime());
                            }
                        }
                    }
                }
                else
                {
                    archerControllerScript[i].B = null;
                }
            }
            else//B = null
            {
                //archer idle animation
                archerAnimatorScript[i].archerState = AnimationArcher.ArcherStates.idle;
            }
        }
    }

    private IEnumerator MoveThroughRoute(GameObject arrow, Vector3[] routePoints)
    {
        for (int i = 0; i < curvePointsTotalCount + 1; i++)
        {
            arrow.transform.LookAt(routePoints[i]);
            yield return StartCoroutine(MoveObject(arrow.transform.position, routePoints[i], arrow));
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
    private IEnumerator CoolDownTime()
    {
        yield return new WaitForSeconds(coolDownTime);
        for (int i = 0; i < totalArcherCount; i++)
        {
            archerControllerScript[i].shootOnce = false;//don't allow shoot to occur even if S is pressed
            archerControllerScript[i].enableLineRenderer = true;//display projectile path again
        }
    }

    private Vector3 Evaluate(float t, Transform A, Transform B, Transform control)//Quadratic Curve functionality
    {
        Vector3 ac = Vector3.Lerp(A.position, control.position, t);//Interpolate from point A to ControlPoint
        Vector3 cb = Vector3.Lerp(control.position, B.position, t);//Interpolate from ControlPoint to Point B

        return Vector3.Lerp(ac, cb, t);
    }
}
