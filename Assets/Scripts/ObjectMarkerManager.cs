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

    void Start()
    {
        mainCamera = Camera.main;
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
                UpdateMarkers();
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void UpdateMarkers()
    {
        if (markers == null)
            return;
        foreach (ObjectMarker marker in markers)
        {
            if (marker.target == null)
                continue;

            Vector3 camTargetVector = (marker.target.position - mainCamera.transform.position);
            float camDistance = camTargetVector.magnitude;
            float camTargetDir = Vector3.Dot(camTargetVector, mainCamera.transform.forward);
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
