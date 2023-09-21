using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class MoveObjectAlongSpline : MonoBehaviour
{
    private SplineContainer spline;  //the spline along which to move which must be in this gameobject
    private float speed = 1f;  //the speed at which the object will move along the spline
    public bool loop;  //should the movement along spline be looped
    private GameObject movingObject;  //the object that will move along the spline
    private SplineAnimate splineAnimator;   //moving object along spline using splineanimate component
    //public bool play;

    void Start()
    {
        spline = GetComponent<SplineContainer>();
        if(movingObject != null)
            SetObjectToMove(movingObject);
    }

    //private void Update()
    //{
    //    if (play)
    //        Move();
    //    else
    //        Stop();
    //}

    public void RemoveExistingMovingObject()
    {   //remove previous object's set components that allowed it to move along spline
        //and remove it from this component's reference
        if (movingObject == null)
            return;
        splineAnimator = null;
        Destroy(movingObject.GetComponent<SplineAnimate>());  //remove spline animate component that was added
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
        splineAnimator.MaxSpeed = speed;
    }
    
    private void SetNewMovingObject(GameObject gameObject)
    {   //setup the gameobject so as to allow it to move along the spline
        movingObject = gameObject;
        splineAnimator = movingObject.AddComponent<SplineAnimate>();
        splineAnimator.Container = spline;
        splineAnimator.AnimationMethod = SplineAnimate.Method.Speed;
        splineAnimator.MaxSpeed = speed;
        if (loop)
            splineAnimator.Loop = SplineAnimate.LoopMode.Loop;
        else
            splineAnimator.Loop = SplineAnimate.LoopMode.Once;
        splineAnimator.PlayOnAwake = false;
    }

    public void SetObjectToMove(GameObject gameObject)
    {
        RemoveExistingMovingObject();
        SetNewMovingObject(gameObject);
    }

    public void Move()
    {   //move the object along the spline
        if(splineAnimator != null)
            splineAnimator.Play();
    }

    public void Stop()
    {   //pause the movement of the object along spline
        if (splineAnimator != null)
            splineAnimator.Pause();
    }

    public void ResetObject(bool autoplay = false)
    {   //reset object to starting position of spline
        if (splineAnimator != null)
            splineAnimator.Restart(autoplay);
    }
}
