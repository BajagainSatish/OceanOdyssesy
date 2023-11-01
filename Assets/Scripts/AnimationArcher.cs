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

        animator.SetBool(aimAtTargetHash,false);
        animator.SetBool(shootAtTargetHash,false);

        archerState = ArcherStates.idle;
    }

    private void Update()
    {
        //Play idle animation/ No animation
        if (archerState == ArcherStates.idle)
        {
            animator.SetBool(aimAtTargetHash, false);
            animator.SetBool(shootAtTargetHash, false);
        }
        else if (archerState == ArcherStates.aim)//Play aim animation
        {
            animator.SetBool(aimAtTargetHash, true);
            animator.SetBool(shootAtTargetHash, false);
        }
        else if (archerState == ArcherStates.shoot)
        {
            animator.SetBool(aimAtTargetHash, false);
            animator.SetBool(shootAtTargetHash, true);
        }
    }
}
