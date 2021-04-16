using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Powers_StickyFoot : MonoBehaviour
{
    public Transform stepPosition;
    public AnimationCurve verticalStepMovement;

    /// <summary>
    /// How far away until the foot can slide
    /// </summary>
    public static float moveThreshold = 2.75f;

    private Quaternion startingRotation;

    private Vector3 plantedPosition;
    private Vector3 prevPlantedPosition;

    private Quaternion plantedRotation = Quaternion.identity;
    private Quaternion prevPlantedRotation = Quaternion.identity;

    private float timeLength = 0.2f;
    private float timeCurrent = 0;

    Transform kneePole;
    public bool footHasMoved = false;

    public bool isAnimating {
        get {
            return (timeCurrent < timeLength);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        kneePole = transform.GetChild(0);
        startingRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimating) { //animation is playing:
            timeCurrent += Time.deltaTime; //move playhead forward

            float p = timeCurrent / timeLength;

            Vector3 finalPosition = Powers_AnimMath.Lerp(prevPlantedPosition, plantedPosition, p);
            finalPosition.y += verticalStepMovement.Evaluate(p);
            transform.position = finalPosition;

            transform.rotation = Powers_AnimMath.Lerp(prevPlantedRotation, plantedRotation, p);
        }
        else
        {
            transform.position = plantedPosition;
            transform.rotation = plantedRotation;
        }
        transform.position = Powers_AnimMath.Slide(transform.position, plantedPosition, 0.001f);
        transform.rotation = Powers_AnimMath.Slide(transform.rotation, plantedRotation, 0.001f);

        Vector3 vFromCenter = transform.position - transform.parent.position;
        vFromCenter.y = 0;
        vFromCenter.Normalize();
        vFromCenter *= 3;
        vFromCenter.y += 2.5f;

        kneePole.position = transform.position + vFromCenter;
    }

    public bool TryToStep() {
        if (isAnimating) return false; //If the foot is still animating, don't step:
        if (footHasMoved) return false;

        Vector3 vBetween = transform.position - stepPosition.position;
        //If too close to prev target, don't try to step:
        if (vBetween.sqrMagnitude < (moveThreshold * moveThreshold)) return false;

        Ray ray = new Ray(stepPosition.position + Vector3.up, Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 3);

        if (Physics.Raycast(ray, out RaycastHit hit, 3))
        {
            //Setup beginning of animation
            prevPlantedPosition = plantedPosition;
            prevPlantedRotation = plantedRotation;

            //Set rotation to starting rotation
            transform.localRotation = startingRotation;

            //Setup end of animation
            plantedPosition = hit.point;
            plantedRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

            //Begin animation
            timeCurrent = 0;

            footHasMoved = true;

            return true;
        }
        else return false;
    }
}
