using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectMarker : MonoBehaviour
{
    public Image marker;
    public Transform target;

    public float showDistanceMin; //distance after which to show icon
    public float showDistanceMax; //distance up to which to show icon
    public bool disableObject; //whether icon should replace the object or not
    public GameObject disablingObject;  //the object to disable
    public float disableDistance; //if replacing the object the from which point should the object be disabled

    private void Awake()
    {
        if (target == null)
            target = transform;
        if (disablingObject == null)
            disablingObject = gameObject;
    }

    public void PlaceMarker()
    {
        marker.transform.position = Camera.main.WorldToScreenPoint(target.position);
        float posx = marker.transform.position.x;
        float posy = marker.transform.position.y;
        if (posx < 0 || posx > Screen.width || posy < 0 || posy > Screen.height) //if not visible in screen disable the marker
            DisableMarker();
        else
            marker.gameObject.SetActive(true);
    }

    public void DisableMarker()
    {
        marker.gameObject.SetActive(false);
    }
}
