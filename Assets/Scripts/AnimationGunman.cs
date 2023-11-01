using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationGunman : MonoBehaviour
{
    private Animator animator;

    private int aimAtTargetHash;
    private int shootAtTargetHash;

    private GunmanController gunmanControllerScript;

    public enum GunmanStates
    {
        idle, aim, shoot
    };
    public GunmanStates gunmanState;

    private void Start()
    {
        animator = GetComponent<Animator>();

        aimAtTargetHash = Animator.StringToHash("aimAtTarget");
        shootAtTargetHash = Animator.StringToHash("shootAtTarget");

        animator.SetBool(aimAtTargetHash, false);
        animator.SetBool(shootAtTargetHash, false);

        gunmanState = GunmanStates.idle;

        gunmanControllerScript = this.GetComponent<GunmanController>();
    }

    private void Update()
    {
        //Play idle animation/ No animation
        if (gunmanState == GunmanStates.idle)
        {
            gunmanControllerScript.rifle.transform.localPosition = new Vector3(-0.125008509f, 0.109148838f, 0.291305363f);
            gunmanControllerScript.rifle.transform.localEulerAngles = new Vector3(341.935364f, 112.180405f, -0.00198736391f);

            animator.SetBool(aimAtTargetHash, false);
            animator.SetBool(shootAtTargetHash, false);
        }
        else if (gunmanState == GunmanStates.aim)//Play aim animation
        {
            gunmanControllerScript.rifle.transform.localPosition = new Vector3(-0.193000004f, 0.307000011f, 0.532999992f);
            gunmanControllerScript.rifle.transform.localEulerAngles = new Vector3(346.360016f, 136.391998f, 1.44500279f);

            animator.SetBool(aimAtTargetHash, true);
            animator.SetBool(shootAtTargetHash, false);
        }
        else if (gunmanState == GunmanStates.shoot)
        {
            animator.SetBool(aimAtTargetHash, false);
            animator.SetBool(shootAtTargetHash, true);
        }
    }
}
