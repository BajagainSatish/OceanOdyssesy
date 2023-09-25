using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEngine.GraphicsBuffer;

public class AnimationStateController : MonoBehaviour
{
    public bool selectedUnitToShoot;

    [SerializeField] private CanonController canonControllerScript;
    [SerializeField] private RopeBurn ropeBurnScript;

    [SerializeField] private GameObject actualCanonLid;
    [SerializeField] private GameObject actualWoodPiece;

    [SerializeField] private float moveTowardsCannonBoxVelocity = 1f;
    [SerializeField] private float moveTowardsCannonVelocity = 1f;
    [SerializeField] private float cannonRotationDuration1 = 0.5f;
    [SerializeField] private float cannonRotationDuration2 = 0.5f;
    [SerializeField] private float cannonRotationDuration3 = 0.5f;
    [SerializeField] private float cannonRotationDuration4 = 0.5f;
    [SerializeField] private GameObject cannonBoxTarget;
    [SerializeField] private float thresholdInitialRot = 0.99f;//lower this value if player is at long distance from cannon or just freezes, for our initial pos 1 works best, lower value has low accuracy

    private Animator animator;
    private int isRunningHash;//used only to improve performance
    private int isPickingHash;//used only to improve performance
    private int isHoldingPickedCannonBall;
    private int isLoadingCannonBallHash;
    private int isLightingTheRopeHash;

    private GameObject animatedCanonLid;
    private GameObject pickedCanonBall;
    private GameObject rig;
    private GameObject headAim;
    private GameObject chestAim;

    private Vector3 startCharacterPos;
    [SerializeField] private Vector3 startCharacterRot;
    private Vector3 pickPos;
    [SerializeField] private Vector3 pickRot;
    [SerializeField] private GameObject playerLoadCannonPosition;
    [SerializeField] private GameObject initialPlayerLocation;
    [SerializeField] private GameObject pickPositionLocation;
    private Vector3 loadCannonPosition;
    [SerializeField] private Vector3 cannonRotation;
    //loadCanonRotation = cannonRot

    [SerializeField] private float pickUpTime = 0.667f;
    [SerializeField] private float loadCannonTime = 1.333f;
    //[SerializeField] private float lightTheRopeTime = 2.16f;//0.7 * 3 = 2.1, approximately equal

    private Vector3 startPos;
    private Quaternion startRotQuaternion;
    private float elapsedTime;
    private enum ProcessingCannonBallStates
    {
        readyToAttack, rotateTowardsCannonBox, moveCharacterTowardsCannonBox, rotateAtCannonBox, playPickAnimation,
        rotateTowardsLoadPosition, moveTowardsLoadPosition, rotateAtLoadPosition, loadCannonBall, lightTheRope
    };
    private ProcessingCannonBallStates currentState;

    private void Start()
    {
        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        isPickingHash = Animator.StringToHash("isPicking");
        isHoldingPickedCannonBall = Animator.StringToHash("pickUpHold");
        isLoadingCannonBallHash = Animator.StringToHash("isLoadingCannonBall");
        isLightingTheRopeHash = Animator.StringToHash("lightTheRope");

        for (int i = 0; i < this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).name == "CanonLid")
            {
                animatedCanonLid = this.transform.GetChild(i).gameObject;
            }
            else if (this.transform.GetChild(i).name == "CanonBall_Picked")
            {
                pickedCanonBall = this.transform.GetChild(i).gameObject;
            }
            else if (this.transform.GetChild(i).name == "Rig")
            {
                rig = this.transform.GetChild(i).gameObject;

                headAim = rig.transform.GetChild(0).gameObject;
                chestAim = rig.transform.GetChild(1).gameObject;
                headAim.GetComponent<MultiAimConstraint>().weight = 0;
                chestAim.GetComponent<MultiAimConstraint>().weight = 0;
            }
        }

        animatedCanonLid.SetActive(false);
        pickedCanonBall.SetActive(false);
        actualCanonLid.SetActive(true);
        actualWoodPiece.SetActive(true);

        startCharacterPos = initialPlayerLocation.transform.position;
        pickPos = pickPositionLocation.transform.position;
        loadCannonPosition = playerLoadCannonPosition.transform.position;//2.3f difference fixed

        transform.position = startCharacterPos;
        transform.eulerAngles = startCharacterRot;

        currentState = ProcessingCannonBallStates.readyToAttack;
        animator.SetLayerWeight(1, 0);//set layer weight for pick run to 0 initially
        //Debug.Log("Start: " + "Simple pick length: " + AnimationLength("SimplePick")); = 0.66667
        //Debug.Log("Start: " + "LoadCannon time length: " + AnimationLength("LoadCanon")); = 1.33333
        //Debug.Log("Start" + "Light animation time length: " + AnimationLength("LightTheRope")); = 2.16
    }
    private void Update()
    {
        pickPos = pickPositionLocation.transform.position;
        loadCannonPosition = playerLoadCannonPosition.transform.position;

        if ((currentState == ProcessingCannonBallStates.readyToAttack) && (selectedUnitToShoot || Input.GetKeyDown(KeyCode.Space)))//will be true only for a single frame
        {
            selectedUnitToShoot = false;
            startPos = transform.position;
            currentState = ProcessingCannonBallStates.rotateTowardsCannonBox;
        }
        if (currentState == ProcessingCannonBallStates.rotateTowardsCannonBox)
        {
            RotateTowardsGameObject(startPos, cannonBoxTarget, ProcessingCannonBallStates.moveCharacterTowardsCannonBox, cannonRotationDuration1);//InitialRotation
        }
        if (currentState == ProcessingCannonBallStates.moveCharacterTowardsCannonBox)
        {
            MoveToPosition(startPos, pickPos, moveTowardsCannonBoxVelocity, isRunningHash, ProcessingCannonBallStates.rotateAtCannonBox);//MoveToPickPosition
        }
        if (currentState == ProcessingCannonBallStates.rotateAtCannonBox)
        {
            startRotQuaternion = transform.rotation;
            RotateTowardsVectorPosition(startRotQuaternion, pickRot, ProcessingCannonBallStates.playPickAnimation, cannonRotationDuration2);//PickRotation
        }
        if (currentState == ProcessingCannonBallStates.playPickAnimation)
        {
            animator.SetBool(isPickingHash, true);
            StartCoroutine(WaitForPick());
        }
        if (currentState == ProcessingCannonBallStates.rotateTowardsLoadPosition)
        {
            startRotQuaternion = transform.rotation;
            RotateTowardsGameObject(pickPos, playerLoadCannonPosition, ProcessingCannonBallStates.moveTowardsLoadPosition, cannonRotationDuration3);//RotateTowardsLoadPosition
        }
        if (currentState == ProcessingCannonBallStates.moveTowardsLoadPosition)
        {
            animator.SetLayerWeight(1, 1);//set layer weight for upper body pick hold animation to play
            MoveToPosition(pickPos, loadCannonPosition, moveTowardsCannonVelocity, isRunningHash, ProcessingCannonBallStates.rotateAtLoadPosition);//MoveToLoadPosition
        }
        if (currentState == ProcessingCannonBallStates.rotateAtLoadPosition)
        {
            animator.SetLayerWeight(1, 0);//set layer weight for pick run to 0
            animator.SetBool(isHoldingPickedCannonBall, true);
            RotateTowardsVectorPosition(startRotQuaternion, cannonRotation, ProcessingCannonBallStates.loadCannonBall, cannonRotationDuration4);//LoadCannonRotation
        }
        if (currentState == ProcessingCannonBallStates.loadCannonBall)
        {
            headAim.GetComponent<MultiAimConstraint>().weight = 1;
            chestAim.GetComponent<MultiAimConstraint>().weight = 0.3f;

            actualCanonLid.SetActive(false);
            animatedCanonLid.SetActive(true);
            pickedCanonBall.SetActive(true);
            animator.SetBool(isLoadingCannonBallHash, true);
            StartCoroutine(WaitForCannonLoad());
        }
        if (currentState == ProcessingCannonBallStates.lightTheRope)
        {
            StartCoroutine(WaitForLightTheRope());
        }
    }
    private IEnumerator WaitForPick()
    {
        pickedCanonBall.SetActive(true);
        yield return new WaitForSeconds(pickUpTime);
        animator.SetBool(isPickingHash, false);
        animator.SetBool(isHoldingPickedCannonBall, true);
        currentState = ProcessingCannonBallStates.rotateTowardsLoadPosition;
    }
    private bool myApproximation(float a, float b)//Mathf.Approximately() didn't work for some cases
    {
        return (Mathf.Abs(a - b) < 0.01f);//tolerance = 0.01f
    }
    private void MoveToPosition(Vector3 startPosition, Vector3 targetPosition, float velocity, int parameterHash, ProcessingCannonBallStates newState)
    {
        float distance = Vector3.Distance(targetPosition, startPosition);
        float duration = distance / velocity;

        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / duration;
        transform.position = Vector3.Lerp(startPosition, targetPosition, percentageComplete);
        animator.SetBool(parameterHash, true);
        if (myApproximation(transform.position.x, targetPosition.x) && myApproximation(transform.position.y, targetPosition.y) && myApproximation(transform.position.z, targetPosition.z))
        {
            elapsedTime = 0;
            currentState = newState;
            animator.SetBool(parameterHash, false);
            return;
        }
    }
    private void RotateTowardsGameObject(Vector3 startPosition, GameObject target, ProcessingCannonBallStates newState, float cannonRotation)
    {
        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / cannonRotation;
        Vector3 dir = target.transform.position - startPosition;//direction to rotate with
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, percentageComplete);

        Vector3 dirFromPersonToCanonBox = (target.transform.position - transform.position).normalized;
        float dotProd = Vector3.Dot(dirFromPersonToCanonBox, transform.forward);

        if (dotProd > thresholdInitialRot)
        {
            elapsedTime = 0;
            startPos = transform.position;
            currentState = newState;
            animator.SetBool(isHoldingPickedCannonBall, false);//not needed for InitialRotation() method but won't make a difference
        }
    }
    private void RotateTowardsVectorPosition(Quaternion startRotation, Vector3 targetRotation, ProcessingCannonBallStates newState, float cannonRotation)
    {
        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / cannonRotation;
        Quaternion targetRotQuaternion = Quaternion.Euler(targetRotation);

        // Use Quaternion.Slerp to smoothly interpolate between the rotations
        transform.rotation = Quaternion.Slerp(startRotation, targetRotQuaternion, percentageComplete);
        if (myApproximation(transform.eulerAngles.y, targetRotation.y))
        {
            elapsedTime = 0;
            currentState = newState;
            animator.SetBool(isHoldingPickedCannonBall, false);
        }
    }
    private IEnumerator WaitForCannonLoad()
    {
        yield return new WaitForSeconds(loadCannonTime);
        pickedCanonBall.SetActive(false);//stop visual movement of cannonball moving from cannon back into cannonbox
        animator.SetBool(isLoadingCannonBallHash, false);
        actualCanonLid.SetActive(true);
        animatedCanonLid.SetActive(false);

        headAim.GetComponent<MultiAimConstraint>().weight = 0;
        chestAim.GetComponent<MultiAimConstraint>().weight = 0;
        //currentState = ProcessingCannonBallStates.rotateTowardsInitialPosition;
        currentState = ProcessingCannonBallStates.lightTheRope;

        canonControllerScript.objectPool_CanonBallScript.noCannonBallSelectedYet = true;//allow choosing of next cannonball
        //This line was specifically kept here because, rather than in the WaitForLightTheRope() or when readyToAttack were true, they would be called continuously in the Update,
        //making the boolean true, so it is made true only after animation is almost completed, preventing multiple cannonballs being shot at once.
    }
    private IEnumerator WaitForLightTheRope()
    {
        actualWoodPiece.SetActive(false);
        animator.SetBool(isLightingTheRopeHash, true);

        yield return new WaitForSeconds(0.7f);//time between lighting and actual fire start to play
        ropeBurnScript.PlayBurnRopeAnimation();

        yield return new WaitForSeconds(0.7f);//time for fire to stop
        ropeBurnScript.StopBurnRopeAnimation();

        yield return new WaitForSeconds(0.7f);//time for animation to end
        actualWoodPiece.SetActive(true);
        animator.SetBool(isLightingTheRopeHash, false);

        currentState = ProcessingCannonBallStates.readyToAttack;

        yield return new WaitForSeconds(0.7f);//time for cannon ball to shoot
        canonControllerScript.ShootCanonBall();
    }
    private float AnimationLength(string name)//was used to find length of animation clip
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == name)
            {
                time = ac.animationClips[i].length;
            }
        }
        return time;
    }
}