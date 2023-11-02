using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationArcher : MonoBehaviour
{
    private Animator animator;

    private int aimAtTargetHash;
    private int shootAtTargetHash;

    public enum ArcherStates { 
        idle,aim,shoot
    };
    public ArcherStates archerState;

    private void Start()
    {
        animator = GetComponent<Animator>();

        aimAtTargetHash = Animator.StringToHash("aimAtTarget");
        shootAtTargetHash = Animator.StringToHash("shootAtTarget");

        archerState = ArcherStates.idle;
    }

    private void Update()
    {
        //Play idle animation/ No animation
        if (archerState == ArcherStates.idle)
        {
            AnimateArcher(false,false);//no aim, no shoot
        }
        else if (archerState == ArcherStates.aim)//Play aim animation
        {
            AnimateArcher(true,false);//aim true, false shoot
        }
        else if (archerState == ArcherStates.shoot)
        {
            AnimateArcher(false,true);//aim false, shoot true
        }
    }
    private void AnimateArcher(bool aimAtOurTarget, bool shootAtOurTarget)
    {
        animator.SetBool(aimAtTargetHash, aimAtOurTarget);
        animator.SetBool(shootAtTargetHash, shootAtOurTarget);
    }
}
