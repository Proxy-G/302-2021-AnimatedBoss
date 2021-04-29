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
    /// An offset bool to use for timing of sin-wave
    /// that controls the walk animation. Measured in radians.
    /// 
    /// Setting to true would add Mathf.PI, or half-a-period.
    /// </summary>
    public bool stepOffset = false;

    private Powers_GuardianController guardian;

    private Vector3 targetPos;
    private Quaternion targetRot;
    
    void Start()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
        guardian = GetComponentInParent<Powers_GuardianController>();
    }

    void Update()
    {
        switch (guardian.state)
        {
            case Powers_GuardianController.States.Idle:
                AnimateIdle(); //Animate the foot idle
                break;
            case Powers_GuardianController.States.Walk:
                AnimateWalk(); //Animate the foot walking
                break;
            case Powers_GuardianController.States.Run:
                AnimateRun(); //Animate the foot run
                break;
            case Powers_GuardianController.States.Jump:
                AnimateJump(); //Animate the foot jump
                break;
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

        if (stepOffset) time = (Time.time + Mathf.PI) * guardian.stepSpeed;
        else time = (Time.time) * guardian.stepSpeed;

        //lateral movement: (x, z)
        float frontToBack = Mathf.Sin(time);
        finalPos += Vector3.Scale(guardian.moveDirAnim, guardian.walkScale) * frontToBack;

        //vertical movement: (y)
        finalPos.y += Mathf.Cos(time) * guardian.walkScale.y;

        //finalPos.x *= goon.walkScale.x;

        bool isOnGround = finalPos.y < startingPos.y;
        if (isOnGround) finalPos.y = startingPos.y;

        //convert from z (-1 to 1) to p (0 to 1 to 0)
        float p = 1 - Mathf.Abs(finalPos.z / guardian.walkScale.x);

        float anklePitch = isOnGround ? 0: p * -10f;

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, finalPos, 0.001f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(anklePitch, 0, 0), 0.001f);
    }

    void FindGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0,0.5f,0), Vector3.down);

        Debug.DrawLine(ray.origin, ray.origin + ray.direction);

        if(Physics.Raycast(ray, out RaycastHit hit, 1f) || Mathf.Abs(transform.localPosition.x - startingPos.x) > 0.25f || Mathf.Abs(transform.localPosition.z - startingPos.z) > 0.25f) //If raycast hits something, set foot to that position
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
    void AnimateRun()
    {
        Vector3 finalPos = startingPos;
        finalPos.x *= .75f;

        float time;

        if (stepOffset) time = (Time.time + Mathf.PI) * guardian.stepSpeed;
        else time = (Time.time) * guardian.stepSpeed;


        //lateral movement: (x, z)
        float frontToBack = Mathf.Sin(time);
        finalPos += Vector3.Scale(guardian.moveDirAnim, guardian.runScale) * frontToBack;

        //vertical movement: (y)
        finalPos.y += Mathf.Cos(time) * guardian.runScale.y;

        //finalPos.x *= goon.walkScale.x;

        bool isOnGround = finalPos.y < startingPos.y;
        if (isOnGround) finalPos.y = startingPos.y;

        //convert from z (-1 to 1) to p (0 to 1 to 0)
        float p = 1 - Mathf.Abs(finalPos.z / guardian.walkScale.x);

        float anklePitch = isOnGround ? 0 : p * -25;

        transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, finalPos, 0.001f);
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(anklePitch, 0, 0), 0.001f);
    }

    void AnimateJump()
    {
        if (stepOffset) transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, startingPos + new Vector3(0, 0.4f, 0.5f), 0.001f);
        else transform.localPosition = Powers_AnimMath.Slide(transform.localPosition, startingPos + new Vector3(0, 0.3f, 0.45f), 0.001f);

        if (stepOffset) transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(32, 0, 0), 0.001f);
        else Powers_AnimMath.Slide(transform.localRotation, startingRot * Quaternion.Euler(26, 0, 0), 0.001f);
    }
}
