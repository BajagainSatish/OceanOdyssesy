using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
public class InstantiateAlongSpline : MonoBehaviour
{
    SplineContainer spline;

    [SerializeField] private GameObject objectToInstantiate;
    [SerializeField] private float raiseHeight;  //units to raise the object up from the base of spline
    [SerializeField] private float spacing;  //spacing between the spawned objects in world units
    private float spacingNormalized; //spacing normalized between 0 and 1
    private float minSpacing = 0.1f;  //used to constrain spacing from becoming zero
    
    
    void Start()
    {
        spline = GetComponent<SplineContainer>();
        spacing = spacing < minSpacing ? minSpacing : spacing;  //constrain spacing from becoming zero
        spacingNormalized = Mathf.Clamp01(spacing / spline.CalculateLength());

        Generate();
    }

    private void Generate()
    {
        float t = 0;
        while(t < 1f)
        {
            float3 position, tangent, upVector;
            spline.Evaluate(t, out position, out tangent, out upVector);
            Vector3 objectPosition = new Vector3(position.x, position.y + raiseHeight, position.z);
            Quaternion objectRotation = Quaternion.LookRotation(tangent, upVector);
            Instantiate(objectToInstantiate, objectPosition, objectRotation, this.transform); //spawn in the evaluate position and parent to this gameobject
            
            t += spacingNormalized;
        }
    }
}
