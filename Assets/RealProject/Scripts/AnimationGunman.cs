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

        gunmanState = GunmanStates.idle;
        gunmanControllerScript = GetComponent<GunmanController>();
    }

    private void Update()
    {
        //Play idle animation/ No animation
        if (gunmanState == GunmanStates.idle)
        {
            //Set appropriate position and rotation for rifle during idle animation
            SetRiflePosition(-0.125008509f, 0.109148838f, 0.291305363f);
            SetRifleEulerAngles(341.935364f, 112.180405f, -0.00198736391f);

            //Neither aim nor shoot, both false
            AnimateGunman(false,false);
        }
        else if (gunmanState == GunmanStates.aim)//Play aim animation
        {
            //Set appropriate position and rotation for rifle during aiming animation
            SetRiflePosition(-0.193000004f, 0.307000011f, 0.532999992f);
            SetRifleEulerAngles(346.360016f, 136.391998f, 1.44500279f);

            //Aim at target, but not shoot
            AnimateGunman(true, false);
        }
        else if (gunmanState == GunmanStates.shoot)
        {
            //Aim = false, Shoot = true
            AnimateGunman(false, true);
        }
    }

    private void SetRiflePosition(float x, float y, float z)
    {
        gunmanControllerScript.rifle.transform.localPosition = new Vector3(x,y,z);
    }
    private void SetRifleEulerAngles(float x, float y, float z)
    {
        gunmanControllerScript.rifle.transform.localEulerAngles = new Vector3(x,y,z);
    }

    private void AnimateGunman(bool aimAtOurTarget, bool shootAtOurTarget)
    {
        animator.SetBool(aimAtTargetHash, aimAtOurTarget);
        animator.SetBool(shootAtTargetHash, shootAtOurTarget);
    }
}
