using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeBurn : MonoBehaviour
{
    private int ropeIsBurningHash;
    private Animator animator;
    private ParticleSystem flameEffect;

    private void Start()
    {
        animator = GetComponent<Animator>();
        ropeIsBurningHash = Animator.StringToHash("burningRope");
        flameEffect = this.transform.GetChild(0).GetComponent<ParticleSystem>();
    }
    public void PlayBurnRopeAnimation()
    {
        flameEffect.Play();
        animator.SetBool(ropeIsBurningHash, true);
    }
    public void StopBurnRopeAnimation()
    {
        flameEffect.Stop();
        animator.SetBool(ropeIsBurningHash, false);
    }
}
