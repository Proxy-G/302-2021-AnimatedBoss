using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_GuardianHipAnimator : MonoBehaviour
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

    Powers_GuardianController guardian;
    
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
        switch (guardian.state)
        {
            case Powers_GuardianController.States.Idle:
                AnimateIdle(); //Animate the hips idle
                break;
            case Powers_GuardianController.States.Walk:
                AnimateWalk(); //Animate the hips walking
                break;
            case Powers_GuardianController.States.Run:
                AnimateRun(); //Animate the hips running
                break;
            case Powers_GuardianController.States.Dead:
                AnimateDead(); //Animate the players death
                break;
        }
    }

    void AnimateIdle()
    {
        float time = Time.time;

        float roll = Mathf.Sin(time) * 3;
        float wiggle = Mathf.Sin(time) * .1f;

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, new Vector3(startingPos.x, startingPos.y - 0.0015f - (wiggle*0.01f), startingPos.z), 0.01f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(-1 - (roll/10), 0, 0), 0.01f);
    }

    void AnimateWalk()
    {
        float time = Time.time * guardian.stepSpeed;
        float roll = Mathf.Sin(time) * rollAmount;
        float hipBounce = Mathf.Cos(time*2);

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, new Vector3(startingPos.x, startingPos.y - 0.001f - (hipBounce * 0.001f), startingPos.z), 0.01f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(-2.5f - (roll/5), 0, roll), 0.01f);
    }

    void AnimateRun()
    {
        float time = Time.time * guardian.stepSpeed;
        float roll = Mathf.Sin(time) * rollAmount;
        float hipBounce = Mathf.Cos(time * 2);

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, new Vector3(startingPos.x, startingPos.y - 0.0035f - (hipBounce * 0.002f), startingPos.z + (.004f)), 0.01f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(-8 - (roll / 3), 0, roll), 0.01f);
    }

    void AnimateDead()
    {
        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, startingPos, 0.01f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot, 0.01f);
    }
}
