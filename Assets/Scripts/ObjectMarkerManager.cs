using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectMarkerManager : MonoBehaviour
{
    public Canvas objectMarkerCanvas;
    ObjectMarker[] markers;

    [SerializeField]float updateInterval = 0.02f;

    Camera mainCamera;

    void Start()
    {
        markers = FindObjectsByType<ObjectMarker>(FindObjectsSortMode.None);
        foreach(ObjectMarker marker in markers)
            marker.marker.transform.SetParent(objectMarkerCanvas.transform);

        StartCoroutine(OnUpdate());
    }

    IEnumerator OnUpdate()
    {
        while (true) 
        {
            mainCamera = Camera.main;
            UpdateMarkers();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void UpdateMarkers()
    {
        foreach (ObjectMarker marker in markers)
        {
            float camDistance = (mainCamera.transform.position - marker.target.position).magnitude;
            if (camDistance > marker.showDistanceMin && camDistance < marker.showDistanceMax)
                marker.PlaceMarker();
            else
                marker.DisableMarker();

            if (marker.disableObject && camDistance > marker.disableDistance)
                marker.disablingObject.SetActive(false);
            else
                marker.disablingObject.SetActive(true);
        }
    }
}
