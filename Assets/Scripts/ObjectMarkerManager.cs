using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ObjectMarkerManager : MonoBehaviour
{
    public Canvas objectMarkerCanvas;
    ObjectMarker[] markers;
    public bool disableUpdate = true;

    [SerializeField]float updateInterval = 0.02f;

    Camera mainCamera;

    void Awake()
    {
        FindMarkersInScene();
        StartCoroutine(OnUpdate());
    }

    public void FindMarkersInScene()
    {
        markers = FindObjectsByType<ObjectMarker>(FindObjectsSortMode.None);
        foreach (ObjectMarker marker in markers)
        {
            marker.marker.transform.SetParent(objectMarkerCanvas.transform);
        }
    }

    IEnumerator OnUpdate()
    {
        while (true) 
        {
            if (!disableUpdate)
            {
                mainCamera = Camera.main;
                UpdateMarkers();
            }
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
