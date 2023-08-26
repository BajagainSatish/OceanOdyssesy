using UnityEngine;
public class Ship : MonoBehaviour
{
    public int id;
    [SerializeField] private Vector3 defaultCameraOffset;
    [SerializeField] private Vector3 defaultCameraRotation;

    [HideInInspector] public Vector3 currentCameraOffset;
    [HideInInspector] public Vector3 currentCameraRotation;
    public float cameraDamping;
    public Transform camTarget;

    private void Awake()
    {
        SetDefaultCameraTargetPosition();
        if(camTarget == null)
            camTarget = transform;
    }

    public void SetDefaultCameraTargetPosition()
    {
        currentCameraOffset = defaultCameraOffset;
        currentCameraRotation = defaultCameraRotation;
    }

    public void SetFromCurrentCameraPosition()
    {
        currentCameraOffset = Camera.main.transform.position - camTarget.position;
        currentCameraRotation = Camera.main.transform.eulerAngles;
    }
}
