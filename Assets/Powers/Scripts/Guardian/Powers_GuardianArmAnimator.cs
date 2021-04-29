using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_GuardianArmAnimator : MonoBehaviour
{
    /// <summary>
    /// Local-space starting position of this object
    /// </summary>
    private Vector3 startingPos;

    /// <summary>
    /// Local-space starting rotation of this object
    /// </summary>
    private Quaternion startingRot;

    /// <summary>
    /// An offset bool to use for timing of sin-wave
    /// that controls the walk animation. Measured in radians.
    /// 
    /// Setting to true would add Mathf.PI, or half-a-period.
    /// </summary>
    public bool stepOffset = false;

    private Powers_GuardianController guardian;
    public Powers_PlayerTargeting playerTargeting;
    public bool canAim = false;

    [Space(10)]

    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;

        guardian = GetComponentInParent<Powers_GuardianController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (guardian.state == Powers_GuardianController.States.Dead) AnimateDead(); //If the player is dead, animate death.
        else if (playerTargeting && playerTargeting.target && playerTargeting.wantsToTarget && canAim) //If the player is targeting...
        {
            AnimateAim(); //Then actively aim the arm to target the object.
        }
        else
        {
            switch (guardian.state)
            {
                case Powers_GuardianController.States.Idle:
                    AnimateIdle(); //Animate the arms idle
                    break;
                case Powers_GuardianController.States.Walk:
                    AnimateWalk(); //Animate the arms walking
                    break;
                case Powers_GuardianController.States.Run:
                    AnimateRun(); //Animate the arms running
                    break;
                case Powers_GuardianController.States.Jump:
                    AnimateJump(); //Animate the arms jump
                    break;
            }
        }
    }

    void AnimateIdle()
    {
        //Override animation to set y-angle at 0. Otherwise, let work as is.
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot, 0.001f);
    }

    void AnimateWalk()
    {
        float time;

        if (stepOffset) time = (Time.time + Mathf.PI) * guardian.stepSpeed;
        else time = (Time.time) * guardian.stepSpeed;

        float sway = Mathf.Sin(time);

        Quaternion finalRot = startingRot;
        if(guardian.moveDirAnim.z > 0) finalRot *= Quaternion.Euler(-sway * 30 + (guardian.moveDirAnim.z * 4), 0, 0);
        else finalRot *= Quaternion.Euler(sway * 30 + (guardian.moveDirAnim.z*4), 0, 0);

        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, finalRot, 0.001f);
    }

    void AnimateAim()
    {

        Vector3 disToTarget = playerTargeting.target.position - transform.position;

        Quaternion targetRotation = Quaternion.LookRotation(disToTarget, Vector3.up);

        Vector3 euler1 = transform.localEulerAngles; //Get local angles BEFORE target rotation
        Quaternion prevRot = transform.rotation; //Save normal rotation
        transform.rotation = targetRotation; //Set rotation
        Vector3 euler2 = transform.localEulerAngles; //Get local angles AFTER target rotation

        if (lockRotationX) euler2.x = euler1.x; //Revert X to prev value
        if (lockRotationY) euler2.y = euler1.y; //Revert Y to prev value
        if (lockRotationZ) euler2.z = euler1.z; //Revert Z to prev value

        euler2.x += 130;

        transform.rotation = prevRot; //revert to normal rotation
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), 0.02f); //animate rotation
    }

    void AnimateRun()
    {
        float time;

        if (stepOffset) time = (Time.time + Mathf.PI) * guardian.stepSpeed;
        else time = (Time.time) * guardian.stepSpeed;

        float sway = Mathf.Sin(time);

        Quaternion finalRot = startingRot;
        if (guardian.moveDirAnim.z > 0) finalRot *= Quaternion.Euler(-sway * 100 - 20, 0, 0);
        else finalRot *= Quaternion.Euler(sway * 100 - 20, 0, 0);

        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, finalRot, 0.001f);
    }

    void AnimateJump()
    {
        if(stepOffset) transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(15, 0, 0), 0.001f);
        else transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(-15, 0, 0), 0.001f);
    }

    void AnimateDead()
    {
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(-340, startingRot.y, startingRot.z), 0.001f);
    }
}
