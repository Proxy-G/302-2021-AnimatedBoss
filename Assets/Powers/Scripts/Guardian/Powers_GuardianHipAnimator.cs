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
                AnimateIdle();
                break;
            case Powers_GuardianController.States.Walk:
                AnimateWalk();
                break;
        }
    }

    void AnimateIdle()
    {
        float time = Time.time;

        float roll = Mathf.Sin(time) * 3;
        float wiggle = Mathf.Sin(time) * .1f;

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, new Vector3(startingPos.x, startingPos.y - 0.001f - (wiggle*0.01f), startingPos.z), 0.01f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(0, 0, -5 + roll), 0.01f);
    }

    void AnimateWalk()
    {
        float time = Time.time * guardian.stepSpeed;
        float roll = Mathf.Sin(time) * rollAmount;

        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(roll, 0, 0), 0.01f);
    }
}
