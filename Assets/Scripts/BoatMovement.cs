using UnityEngine;

public class BoatMovement : MonoBehaviour
{
    [SerializeField] private float forwardMovementPower = 700f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float drag = 10f;
    [SerializeField] private float rotationSpeed = 0.5f;
    public Joystick joystick;

    private Rigidbody _rigidbody;
    private bool movingForward;
    private float moveHorizontal;
    private float moveVertical;
    private Quaternion targetRotation;

    public GameController gameController;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (gameController.playIsClicked)
        {
            moveHorizontal = joystick.Horizontal;
            moveVertical = joystick.Vertical;

            //actual forward movement throughout the entire game
            ApplyForceToReachVelocity(_rigidbody, transform.forward * maxSpeed, forwardMovementPower);

            // moving forward check
            // if the y-component of the cross product of boat's forward direction and its velocity is negative, boat is moving forward
            movingForward = Vector3.Cross(transform.forward, _rigidbody.velocity).y < 0;

            //adjust boat velocity to ensure it moves in correct direction
            //rotate velocity vector based on boat's movement direction
            _rigidbody.velocity = Quaternion.AngleAxis(Vector3.SignedAngle(_rigidbody.velocity, (movingForward ? 1f : 0f) * transform.forward, Vector3.up) * drag, Vector3.up) * _rigidbody.velocity;

            //Boat Rotation
            Vector2 input = new Vector2(moveHorizontal, moveVertical);
            Vector2 inputDir = input.normalized;
            if (inputDir != Vector2.zero)//persist in latest rotated direction
            {
                targetRotation = Quaternion.Euler(Vector3.up * Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg);

                // Interpolate the rotation using Quaternion.Slerp
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
    private void ApplyForceToReachVelocity(Rigidbody rigidbody, Vector3 velocity, float force = 1, ForceMode mode = ForceMode.Force)
    {
        //This function applies force to the boat's rigidbody to reach the desired target velocity
        if (force == 0 || velocity.magnitude == 0)//means no force needs to be applied
            return;

        velocity += velocity.normalized * 0.2f * rigidbody.drag;

        //force = 1 => need 1 s to reach velocity (if mass is 1) => force can be max 1 / Time.fixedDeltaTime
        force = Mathf.Clamp(force, -rigidbody.mass / Time.fixedDeltaTime, rigidbody.mass / Time.fixedDeltaTime);

        //dot product is a projection from rhs to lhs with a length of result / lhs.magnitude
        if (rigidbody.velocity.magnitude == 0)
        {
            rigidbody.AddForce(velocity * force, mode);
        }
        else
        {
            var velocityProjectedToTarget = (velocity.normalized * Vector3.Dot(velocity, rigidbody.velocity) / velocity.magnitude);
            rigidbody.AddForce((velocity - velocityProjectedToTarget) * force, mode);
        }
    }
}
