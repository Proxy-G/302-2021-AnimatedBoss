using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_HamsterHeadAnimator : MonoBehaviour
{
    /// <summary>
    /// Local-space starting position of this object
    /// </summary>
    private Vector3 startingPos;
    /// <summary>
    /// Local-space starting rotation of this object
    /// </summary>
    private Quaternion startingRot;

    Quaternion targetRot;

    private Powers_HamsterAI hamster;
    private float headRotate = 0;
    private bool attackPhase1 = false;
    private float attackPhase2 = 1f;

    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        hamster = GetComponentInParent<Powers_HamsterAI>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (hamster.state)
        {
            case Powers_HamsterAI.States.Idle:
                AnimateLook(); //Animate the head idle
                break;
            case Powers_HamsterAI.States.Walk:
                AnimateLook(); //Animate the head walking
                break;
            case Powers_HamsterAI.States.Attack:
                AnimateAttack(); //Animate the head attacking
                break;
            case Powers_HamsterAI.States.Dead:
                AnimateDead(); //Animate the hamster's death
                break;
        }
    }

    void AnimateLook()
    {
        //Get the rotation needed to rotate head towards the player
        Quaternion initialRot = transform.rotation;
        transform.LookAt(hamster.playerPos.transform, Vector3.up);
        targetRot = transform.localRotation;
        transform.rotation = initialRot;

        targetRot.x = Mathf.Clamp(targetRot.x, Mathf.Deg2Rad * -12, Mathf.Deg2Rad * 12);
        targetRot.y = Mathf.Clamp(targetRot.y, Mathf.Deg2Rad * -20, Mathf.Deg2Rad * 20);
        targetRot.z = Mathf.Clamp(targetRot.z, Mathf.Deg2Rad * -5, Mathf.Deg2Rad * 5);

        //slide local rotation to target rot
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, targetRot, 0.01f);

        Vector3 eulerAnglesClamp = transform.localEulerAngles;

        //Clamp the euler angles on the x-axis.
        if (eulerAnglesClamp.x < 180) eulerAnglesClamp.x = Mathf.Clamp(eulerAnglesClamp.x, 0, 15);
        else eulerAnglesClamp.x = Mathf.Clamp(eulerAnglesClamp.x, 345, 360);

        //Clamp the euler angles on the y-axis.
        if (eulerAnglesClamp.y < 180) eulerAnglesClamp.y = Mathf.Clamp(eulerAnglesClamp.y, 0, 40);
        else eulerAnglesClamp.y = Mathf.Clamp(eulerAnglesClamp.y, 320, 360);

        //Clamp the euler angles on the z-axis.
        if (eulerAnglesClamp.z < 180) eulerAnglesClamp.z = Mathf.Clamp(eulerAnglesClamp.z, 0, 5);
        else eulerAnglesClamp.z = Mathf.Clamp(eulerAnglesClamp.z, 355, 360);

        //Set the euler angles with the new clamped one.
        transform.localEulerAngles = eulerAnglesClamp;
    }

    void AnimateAttack()
    {      
        if(headRotate <= -29.8f || attackPhase1) 
        {
            attackPhase1 = true;
            attackPhase2 -= Time.deltaTime; //Countdown till head smashes down

            if (headRotate > 32.8f) //Once head has smashed down, anim is complete and it's time to do damage
            {
                attackPhase1 = false;
                attackPhase2 = 1f;
                headRotate = 0;

                hamster.doDamage = true;
            }
            else if(attackPhase2 < 0) headRotate = Powers_AnimMath.Slide(headRotate, 35, 0.001f); //Wait until the countdown is complete before smashing head down
        }
        else headRotate = Powers_AnimMath.Slide(headRotate, -30, 0.01f); //Rotate head up doing first phase

        //Rotate the head
        if(!hamster.doDamage) transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(headRotate, 0, 0), 0.01f);
    }

    void AnimateDead()
    {
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(0, -6, -11), 0.001f);
    }
}
