using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_GuardianLegAnimator : MonoBehaviour
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
        if(guardian.moveDirAnim.z > 0.2f || guardian.moveDirAnim.z < -0.2f)
        {
            Quaternion finalRot = startingRot;
            if (guardian.moveDirAnim.z > 0.2f) finalRot *= Quaternion.Euler(0, 10 * guardian.moveDirAnim.x, 0);
            else finalRot *= Quaternion.Euler(0, -10 * guardian.moveDirAnim.x, 0);

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
