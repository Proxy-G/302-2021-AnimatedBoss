using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_GuardianBodyAnimator : MonoBehaviour
{
    /// <summary>
    /// Local-space starting position of this object
    /// </summary>
    private Vector3 startingPos;

    /// <summary>
    /// Local-space starting rotation of this object
    /// </summary>
    private Quaternion startingRot;

    private Powers_GuardianController guardian;
    public Powers_PlayerTargeting playerTargeting;

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
        if (playerTargeting && playerTargeting.target && playerTargeting.wantsToTarget) //If the player is targeting...
        {
            AnimateAim(); //Then actively aim the arm to target the object.
        }
        else
        {
            if (guardian.moveDirAnim.z > 0.2f || guardian.moveDirAnim.z < -0.2f)
            {
                Quaternion finalRot = startingRot;
                if (guardian.moveDirAnim.z > 0.2f) finalRot *= Quaternion.Euler(0, 33 * guardian.moveDirAnim.x, 0);
                else finalRot *= Quaternion.Euler(0, -20 * guardian.moveDirAnim.x, 0);

                //Override animation to set y-angle at 0. Otherwise, let work as is.
                transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, finalRot, 0.001f);
            }
            else
            {
                //Override animation to set y-angle at 0. Otherwise, let work as is.
                transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, startingRot, 0.001f);
            }
        }
    }

    void AnimateNorm()
    {

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

        transform.rotation = prevRot; //revert to normal rotation
        transform.localRotation = Powers_AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), 0.02f); //animate rotation
    }
}
