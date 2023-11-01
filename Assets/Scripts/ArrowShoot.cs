using System.Collections;
using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    [SerializeField] private ObjectPool_Projectile objectPoolArrowScript;
    [SerializeField] private float lineWidth;
    [SerializeField] private float arrowVelocity;
    [SerializeField] private float leastDistanceForStraightHit;
    [SerializeField] private float adjustCurveAngle;
    public float archerMaxRange;
    [SerializeField] private float coolDownTime;

    //[SerializeField] private float upperYLimit = 0.7f;
    //[SerializeField] private float lowerYLimit = -0.1f;
    //[SerializeField] private float archerShootAngleRange = 80;

    public static int curvePointsTotalCount = 20;//change this value to change the number of points in curve, and control smoothness of curve by increasing the number
    public static int totalArcherCount = 8;

    private GameObject arrow;
    private Vector3 myShipPosition;
    private GameObject scaleFactorGameObject;
    private GameObject myShipCenter;
    private GameObject archerParentObject;
    private GameObject[] archers = new GameObject[totalArcherCount];
    private ArcherController[] archerControllerScript = new ArcherController[totalArcherCount];
    private AnimationArcher[] archerAnimatorScript = new AnimationArcher[totalArcherCount];

    private float adjustDistanceFactor;

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
                myShipCenter = gameObject;
            }
        }
        for (int i = 0; i < scaleFactorGameObject.transform.childCount; i++)
        {
            GameObject gameObject = scaleFactorGameObject.transform.GetChild(i).gameObject;
            if (gameObject.name == "Archers")
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
            archerControllerScript[i].enableLineRenderer = true;
        }
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

                //bool withinArcherRotateRange = archerControllerScript[i].withinArcherRotateRange;
                LineRenderer lineRenderer = archerControllerScript[i].lineRenderer;
                bool shootOnce = archerControllerScript[i].shootOnce;               
                Vector3[] routePoints = archerControllerScript[i].routePoints;

                //float distance = Mathf.Sqrt((B.position.x - shipPosition.x) * (B.position.x - shipPosition.x) + (B.position.y - shipPosition.y) * (B.position.y - shipPosition.y) + (B.position.z - shipPosition.z) * (B.position.z - shipPosition.z));
                float distance = Vector3.Distance(B.position,myShipPosition);

                /*
                Vector3 difference = B.position - A.position;
                Vector3 targetDirection = (B.position - A.position).normalized;
                Vector3 archersForwardDirection = archerControllerScript[i].transform.forward;

                // Calculate the angle between the forward direction and the target direction
                //float angle = Vector3.Angle(targetDirection, archersForwardDirection);

                //Initially, we had constraints assuming archer didn't rotate towards target at all times and archer faced only one side of ship at a time
                //If archer rotates towards target at all times, we can remove these 3 following constraints, we don't need withinArcherRotateRange variable, and can safely remove it

                // Check if the angle is within the desired range
                if (angle <= archerShootAngleRange && difference.y >= lowerYLimit && difference.y <= upperYLimit)
                {
                    withinArcherRotateRange = true;
                }
                else
                {
                    withinArcherRotateRange = false;
                }
                */
                //if (distance <= archerMaxRange && withinArcherRotateRange)
                if (distance < archerMaxRange)
                {
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
                                archerControllerScript[i].enableLineRenderer = false;
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
    private IEnumerator CoolDownTime()
    {
        yield return new WaitForSeconds(coolDownTime);
        for (int i = 0; i < totalArcherCount; i++)
        {
            archerControllerScript[i].shootOnce = false;
            archerControllerScript[i].enableLineRenderer = true;
        }
    }

    private Vector3 Evaluate(float t, Transform A, Transform B, Transform control)//Quadratic Curve functionality
    {
        Vector3 ac = Vector3.Lerp(A.position,control.position,t);//Interpolate from point A to ControlPoint
        Vector3 cb = Vector3.Lerp(control.position, B.position, t);//Interpolate from ControlPoint to Point B

        return Vector3.Lerp(ac,cb,t);
    }
}
