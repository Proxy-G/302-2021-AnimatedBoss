using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script animates the foots and legs by
/// changing the local position of this object (ik target).
/// </summary>
public class Powers_HamsterFootAnimator : MonoBehaviour
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

    public bool backFoot = false;  

    private Powers_HamsterAI hamster;
    private float footAttack = 0;
    private bool attackPhase1 = false;
    private float attackPhase2 = 1f;

    void Start()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        hamster = GetComponentInParent<Powers_HamsterAI>();
    }

    void Update()
    {
        switch (hamster.state)
        {
            case Powers_HamsterAI.States.Idle:
                AnimateIdle(); //Animate the foot idle
                break;
            case Powers_HamsterAI.States.Walk:
                AnimateWalk(); //Animate the foot walking
                break;
            case Powers_HamsterAI.States.Attack:
                AnimateAttack(); //Animate the foot idle
                break;
            case Powers_HamsterAI.States.Hitting:
                AnimateIdle(); //Animate the foot idle
                break;
            case Powers_HamsterAI.States.Dead:
                AnimateDead(); //Animate the hamster's death
                break;
        }

        if (hamster.doDamage) //Reset attack variables once hamster attack anim is complete
        {
            attackPhase1 = false;
            attackPhase2 = 1f;
            footAttack = 0;
        }
    }

    void AnimateIdle()
    {
        FindGround();
    }

    void AnimateWalk()
    {
        Vector3 finalPos = startingPos;
        finalPos.x *= .75f;

        float time;

        if (stepOffset) time = (Time.time + Mathf.PI) * hamster.stepSpeed;
        else time = (Time.time) * hamster.stepSpeed;

        //lateral movement: (x, z)
        float frontToBack = Mathf.Sin(time);
        finalPos += hamster.walkScale * frontToBack;

        //vertical movement: (y)
        finalPos.y += Mathf.Cos(time) * hamster.walkScale.y;

        bool isOnGround = finalPos.y < startingPos.y - 0.05f;
        if (isOnGround) finalPos.y = startingPos.y - 0.05f;

        //convert from z (-1 to 1) to p (0 to 1 to 0)
        float p = 1 - Mathf.Abs(finalPos.z / hamster.walkScale.z);

        float anklePitch = isOnGround ? 0: p * -7.5f;

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, finalPos, 0.001f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(0, 0, anklePitch), 0.001f);
    }

    void FindGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, 2f, 0), Vector3.down);

        Debug.DrawLine(ray.origin, ray.origin + ray.direction);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f)) //If raycast hits something, set foot to that position
        {
            transform.position = Powers_AnimMath.Slide(transform.position, hit.point, 0.001f);
            transform.rotation = Powers_AnimMath.Slide(transform.rotation, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation, 0.001f);
        }
        else //Otherwise, set foot to default position
        {
            transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, startingPos, 0.001f);
            transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot, 0.001f);
        }

    }

    void AnimateAttack()
    {
        if (backFoot) FindGround(); 
        else
        {
            float time = Time.time*2;
            float raise = Mathf.Sin(time);

            transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, startingPos + new Vector3(0, Mathf.Clamp(raise, 0, raise), 0), 0.01f);
        }

        if (backFoot) FindGround(); //If back feet, keep grounding...
        if (footAttack > 0.99f || attackPhase1)
        {
            attackPhase1 = true;
            attackPhase2 -= Time.deltaTime; //Countdown till head smashes down

            if (attackPhase2 < 0) footAttack = Powers_AnimMath.Slide(footAttack, 0, 0.001f); //Wait until the countdown is complete before smashing head down
        }
        else footAttack = Powers_AnimMath.Slide(footAttack, 1, 0.01f); //Rotate head up doing first phase

        //Rotate the head
        if (!hamster.doDamage) transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, startingPos + new Vector3(0,1,0), 0.01f);
    }

    void AnimateDead()
    {
        if(!backFoot) transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, startingPos + new Vector3(0, 2, 9), 0.01f);
        else transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, startingPos + new Vector3(0, 8, -18), 0.01f);
        
        if(!backFoot) transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(0, 0, 60), 0.01f);
        else transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(0, 0, -120), 0.01f);
    }
}
