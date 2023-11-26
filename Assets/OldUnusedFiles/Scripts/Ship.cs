using UnityEngine;

[RequireComponent(typeof(CameraFollowerTarget))]
public class Ship : MonoBehaviour
{
    public int id;
    [HideInInspector] public CameraFollowerTarget cameraTarget;
    public float speed = 1f;

    private void Start()
    {
        cameraTarget = GetComponent<CameraFollowerTarget>();
    }
}
