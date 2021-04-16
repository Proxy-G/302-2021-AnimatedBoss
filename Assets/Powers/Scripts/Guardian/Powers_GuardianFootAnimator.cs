using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script animates the foots and legs by
/// changing the local position of this object (ik target).
/// </summary>
public class Powers_GuardianFootAnimator : MonoBehaviour
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
    /// An offset value to use for timing of sin-wave
    /// that controls the walk animation. Measured in radians.
    /// 
    /// A value of Mathf.PI  would be half-a-period.
    /// </summary>
    public float stepOffset = 0;

    private Powers_GuardianController goon;

    private Vector3 targetPos;
    private Quaternion targetRot;
    
    void Start()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        goon = GetComponentInParent<Powers_GuardianController>();
    }

    void Update()
    {
        switch (goon.state)
        {
            case Powers_GuardianController.States.Idle:
                AnimateIdle();
                break;
            case Powers_GuardianController.States.Walk:
                AnimateWalk();
                break;
        }

        //ease position and rotation towards targets:
        //transform.position = Powers_AnimMath.Slide(transform.position, targetPos, 0.001f);
        //transform.rotation = Powers_AnimMath.Slide(transform.rotation, targetRot, 0.001f);
    }

    void AnimateIdle()
    {
        
        //targetPos = transform.TransformPoint(startingPos);
        //targetRot = transform.parent.rotation * startingRot;

        FindGround();
    }

    void AnimateWalk()
    {
        Vector3 finalPos = startingPos;

        float time = (Time.time + stepOffset) * goon.stepSpeed;

        //lateral movement: (x, z)
        float frontToBack = Mathf.Sin(time);
        finalPos += goon.moveDir * frontToBack * goon.walkScale.x;

        //vertical movement: (y)
        finalPos.y += Mathf.Cos(time) * goon.walkScale.y;

        //finalPos.x *= goon.walkScale.x;

        bool isOnGround = finalPos.y < startingPos.y;
        if (isOnGround) finalPos.y = startingPos.y;

        //convert from z (-1 to 1) to p (0 to 1 to 0)
        float p = 1 - Mathf.Abs(finalPos.z / goon.walkScale.z);

        float anklePitch = isOnGround ? 0: p * -35;

        //targetPos = transform.TransformPoint(finalPos);
        //targetRot = transform.parent.rotation * (startingRot * Quaternion.Euler(0, 0, anklePitch));

        transform.localPosition = finalPos;
        transform.localRotation = startingRot * Quaternion.Euler(anklePitch, 0, 0);
    }

    void FindGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0,0.5f,0), Vector3.down);

        Debug.DrawLine(ray.origin, ray.direction);

        if(Physics.Raycast(ray, out RaycastHit hit, 1.3f))
        {
            transform.position = hit.point;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else
        {

        }
        
    }
}
