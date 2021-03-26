using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_HipAnimator : MonoBehaviour
{
    /// <summary>
    /// Local-space starting rotation of this object
    /// </summary>
    private Quaternion startingRot;
    public float rollAmount = 2;

    Powers_GoonController goon;
    
    // Start is called before the first frame update
    void Start()
    {
        startingRot = transform.localRotation;
        goon = GetComponentInParent<Powers_GoonController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (goon.state)
        {
            case Powers_GoonController.States.Idle:
                AnimateIdle();
                break;
            case Powers_GoonController.States.Walk:
                AnimateWalk();
                break;
        }
    }

    void AnimateIdle()
    {
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot, 0.01f);
    }

    void AnimateWalk()
    {
        float time = Time.time * goon.stepSpeed;
        float roll = Mathf.Sin(time) * rollAmount;

        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(0, 0, roll), 0.01f);
    }
}
