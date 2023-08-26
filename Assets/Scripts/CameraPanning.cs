using System.Collections;
using UnityEngine;

public class CameraPanning : MonoBehaviour
{
    [Range(0f, 1f)][SerializeField] private float touchAreaMinX = 0f;
    [Range(0f, 1f)][SerializeField] private float touchAreaMaxX = 1f;
    [Range(0f, 1f)][SerializeField] private float touchAreaMinY = 0.5f;
    [Range(0f, 1f)][SerializeField] private float touchAreaMaxY = 1f;

    [SerializeField] private float panSpeed = 0.01f;

    private Touch prevTouch;
    private int prevTouchCount = 0;
    private bool touchSet = false;
    public bool isPanning;

    private RectBound touchArea;


    private void Start()
    {
        touchArea = new RectBound(Screen.width * touchAreaMinX, Screen.width * touchAreaMaxX,
                                  Screen.height * touchAreaMinY, Screen.height * touchAreaMaxY);
        StartCoroutine(SetTouchArea());
    }

    IEnumerator SetTouchArea()
    {
        yield return new WaitForSeconds(0.1f);
        touchArea = new RectBound(Screen.width * touchAreaMinX, Screen.width * touchAreaMaxX,
                                  Screen.height * touchAreaMinY, Screen.height * touchAreaMaxY);
    }

    void Update()
    {
        isPanning = false;
        if (Input.touchCount == 1)
        {
            Touch curTouch = Input.GetTouch(0);
            if (curTouch.phase == TouchPhase.Began || curTouch.phase == TouchPhase.Stationary || prevTouchCount != 1)
            {
                prevTouch = curTouch;
            }
            else
            {
                touchArea = new RectBound(Screen.width * touchAreaMinX, Screen.width * touchAreaMaxX,
                                  Screen.height * touchAreaMinY, Screen.height * touchAreaMaxY);
                bool touchWithinArea = touchArea.Contains(curTouch.position.x, curTouch.position.y);
                if (!touchSet && touchWithinArea)
                    touchSet = true;
                else if (touchSet && !touchWithinArea)
                    touchSet = false;
                else if (touchSet && touchWithinArea)
                {
                    isPanning = true;
                    HandlePanning(curTouch);
                }
            }
        }
        else { touchSet = false; }

        prevTouchCount = Input.touchCount;
    }

    void HandlePanning(Touch curTouch)
    {
        Vector2 deltaPos = prevTouch.position - curTouch.position;
        transform.position +=  (deltaPos.x * transform.right + deltaPos.y * transform.up) * panSpeed;
    }
}
