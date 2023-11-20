using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BuoyancyObject : MonoBehaviour
{
    [SerializeField] private Transform[] floaters;//for a simple object, just add the object itself, but for a more complex object assign specific floating points(empty gameobjects) for gameobject wherever necessary
    [SerializeField] private float underWaterDrag = 3f;
    [SerializeField] private float underWaterAngularDrag = 1f;
    [SerializeField] private float airDrag = 0f;
    [SerializeField] private float airAngularDrag = 0.05f;
    [SerializeField] private float floatingPower = 500f;
    [SerializeField] private float waterHeight = 0f;

    private Rigidbody m_Rigidbody;
    private int floatersUnderWater;
    private bool underwater;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        floatersUnderWater = 0;
        for (int i = 0; i < floaters.Length; i++)
        {
        float difference = floaters[i].position.y - waterHeight;

        if (difference < 0) {//floating point is underwater 
            m_Rigidbody.AddForceAtPosition(Vector3.up * floatingPower * Mathf.Abs(difference), floaters[i].position, ForceMode.Force);//add upthrust, more the depth(difference) higher the upthrust
            floatersUnderWater += 1;
            if (!underwater) {
                underwater = true;
                SwitchState(true);
            }
        }
        }
        if (underwater && floatersUnderWater == 0) {
            underwater = false;
            SwitchState(false);
        }
    }

    void SwitchState(bool isUnderWater) {
        if (isUnderWater)
        {
            m_Rigidbody.drag = underWaterDrag;
            m_Rigidbody.angularDrag = underWaterAngularDrag;
        }
        else
        {
            m_Rigidbody.drag = airDrag;
            m_Rigidbody.angularDrag = airAngularDrag;
        }
    }
}
