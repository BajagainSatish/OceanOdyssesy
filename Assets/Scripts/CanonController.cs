using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonController : MonoBehaviour
{
    public GameObject Canonball;
    public Transform ShootPoint;
    [SerializeField] private float blastPower;
    public ObjectPool_CanonBall objectPool_CanonBallScript;
    private ParticleSystem smokeParticleEffect;
    private void Awake()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).name == "ObjectPool_CanonBalls")
            {
            objectPool_CanonBallScript = this.transform.GetChild(i).GetComponent<ObjectPool_CanonBall>();
            }
            else if (this.transform.GetChild(i).name == "SmokeParticleEffects")
            {
                GameObject particleEffectsObject = this.transform.GetChild(i).gameObject;
                smokeParticleEffect = particleEffectsObject.transform.GetChild(0).GetComponent<ParticleSystem>();               
            }
        }
    }

    public void ShootCanonBall()
    {
        GameObject newCanonBall = objectPool_CanonBallScript.ReturnCanonBall();
        if (newCanonBall != null)
        {
            newCanonBall.transform.position = ShootPoint.position;
            newCanonBall.SetActive(true);
            newCanonBall.GetComponent<Rigidbody>().velocity = ShootPoint.transform.forward * blastPower;
            smokeParticleEffect.Play();      
        //Initially x-rotation of shootpoint set to -10, tweak it as necessary
        }
    }
}
