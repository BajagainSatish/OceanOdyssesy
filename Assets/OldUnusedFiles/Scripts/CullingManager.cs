using System.Collections;
using UnityEngine;

public class CullingManager : MonoBehaviour
{
    GameObject[] cullingObjects;
    CullingObject[] cullingComponents;
    float updateInterval = 0.01f;

    public float viewRadius = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        cullingComponents = FindObjectsByType<CullingObject>(FindObjectsSortMode.None);
        cullingObjects = new GameObject[cullingComponents.Length];
        for(int i= 0; i < cullingComponents.Length; i++)
        {
            cullingObjects[i] = cullingComponents[i].gameObject;
        }

        StartCoroutine(Cull());
    }

    IEnumerator Cull()
    {
        while(true)
        {
            DistanceCulling();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void DistanceCulling()
    {
        Vector3 camerapos = Camera.main.transform.position;
        int i = 0;
        foreach(GameObject cullingObj in cullingObjects)
        {
            float cullingDistance = cullingComponents[i].boundingRadius + this.viewRadius;
            
            if((camerapos - cullingObj.transform.position).magnitude > cullingDistance)
                cullingObj.SetActive(false);
            else
                cullingObj.SetActive(true);
            i++;
        }
    }
}
