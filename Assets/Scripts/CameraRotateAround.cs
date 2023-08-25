using UnityEngine;

public class CameraRotateAround : MonoBehaviour
{
    public Transform target;
    public float verticalRotationLimitMin = 20f;
    public float verticalRotationLimitMax = 70f;
    private float rotateFactor = 10f;

    private Touch prevTouch;
    private int prevTouchCount = 0;

    private Vector3 midAxisStart; //the mid axis in screen space used for rotation
    private Vector3 midAxisEnd;

    private void Start()
    {
        midAxisStart = new Vector3(Screen.width * 0.25f, Screen.height * 0.5f, 1f); //the z-value doesn't matter for our purpose it is just because it is required by the function
        midAxisEnd = new Vector3(Screen.width * .75f, Screen.height * 0.5f, 1f);
        transform.LookAt(target);
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch curTouch = Input.GetTouch(0);
            if (curTouch.phase == TouchPhase.Began || curTouch.phase == TouchPhase.Stationary || prevTouchCount != 1)
            {
                prevTouch = curTouch;
            }
            else
            {
                HandleLookAt(curTouch);
            }
        }

        prevTouchCount = Input.touchCount;
    }

    void HandleLookAt(Touch curTouch)
    {
        Vector2 deltaPos = prevTouch.position - curTouch.position;

        //use vertical delta movement to move camera around aixs parallel to midaxis of the screen
        //and passing through target point
        float verticalMoveAngle = -deltaPos.y * Time.deltaTime * rotateFactor;
        if (CheckVerticalRotationLimits(verticalMoveAngle))
        {
            Vector3 verticalRotationAxisStart = Camera.main.ScreenToWorldPoint(midAxisStart);
            Vector3 verticalRotationAxisEnd = Camera.main.ScreenToWorldPoint(midAxisEnd);
            Vector3 verticalRotationAxis = verticalRotationAxisEnd - verticalRotationAxisStart;
            transform.RotateAround(target.position, -verticalRotationAxis, verticalMoveAngle);
        }

        //use horizantal delta movement to move camera around world-up axis
        //and passing through target point
        float horizantalMoveAngle = - deltaPos.x * Time.deltaTime * rotateFactor;
        transform.RotateAround(target.position, Vector3.up, horizantalMoveAngle);

        transform.LookAt(target);

        prevTouch = curTouch;
    }

    private bool CheckVerticalRotationLimits(float deltaMoveAngle)
    {//return false if rotation changes limit of the object
       float verticalAngle = Vector3.Angle(transform.position - target.position, Vector3.up);
        if (deltaMoveAngle > 0 && verticalAngle < verticalRotationLimitMax)
            return true;
        if(deltaMoveAngle < 0 && verticalAngle > verticalRotationLimitMin)
            return true;
        return false;
    }
}
