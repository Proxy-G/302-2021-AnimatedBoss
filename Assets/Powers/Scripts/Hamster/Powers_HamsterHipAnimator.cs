using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_HamsterHipAnimator : MonoBehaviour
{
    /// <summary>
    /// Local-space starting position of this object
    /// </summary>
    private Vector3 startingPos;
    /// <summary>
    /// Local-space starting rotation of this object
    /// </summary>
    private Quaternion startingRot;
    public float rollAmount = 2;

    private Powers_HamsterAI hamster;
    private float hipRotate = 0;
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
                AnimateIdle(); //Animate the hips idle
                break;
            case Powers_HamsterAI.States.Walk:
                AnimateWalk(); //Animate the hips walking
                break;
            case Powers_HamsterAI.States.Attack:
                AnimateAttack(); //Animate the hips attacking
                break;
            case Powers_HamsterAI.States.Dead:
                AnimateDead(); //Animate the players death
                break;
        }

        if (hamster.doDamage) //Reset attack variables once hamster attack anim is complete
        {
            attackPhase1 = false;
            attackPhase2 = 1f;
            hipRotate = 0;
        }
    }

    void AnimateIdle()
    {
        float time = Time.time;

        float roll = Mathf.Sin(time) * 3;
        float wiggle = Mathf.Sin(time) * .1f;

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, new Vector3(startingPos.x, startingPos.y - 0.005f - (wiggle * 0.002f), startingPos.z), 0.01f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(-1 - (roll / 10), 0, 0), 0.01f);
    }

    void AnimateWalk()
    {
        float time = Time.time * hamster.stepSpeed;
        float roll = Mathf.Sin(time) * rollAmount;
        float hipBounce = Mathf.Cos(time * 2);

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, new Vector3(startingPos.x, startingPos.y - 0.005f - (hipBounce * 0.003f), startingPos.z), 0.01f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(-2.5f - (roll / 5), 0, roll), 0.01f);
    }

    void AnimateAttack()
    {
        if (hipRotate <= -4.9f || attackPhase1)
        {
            attackPhase1 = true;
            attackPhase2 -= Time.deltaTime; //Countdown till head smashes down

            if (attackPhase2 < 0) hipRotate = Powers_AnimMath.Slide(hipRotate, 5, 0.001f); //Wait until the countdown is complete before smashing head down
        }
        else hipRotate = Powers_AnimMath.Slide(hipRotate, -5, 0.01f); //Rotate head up doing first phase

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, new Vector3(startingPos.x, startingPos.y - 0.0015f, startingPos.z), 0.01f);
        //Rotate the hips
        if (!hamster.doDamage) transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(hipRotate, 0, 0), 0.01f);
    }

    void AnimateDead()
    {
        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, new Vector3(startingPos.x, startingPos.y-.05275f, startingPos.z), 0.01f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot, 0.01f);
    }
}
