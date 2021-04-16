using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_ScorpionController : MonoBehaviour
{
    public Transform groundRing;
    public List<Powers_StickyFoot> feet = new List<Powers_StickyFoot>();
    CharacterController pawn;

    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();

        for (int i = feet.Count - 1; i >= 0; i--)
        {
            if (i % 2 == 0) feet[i].footHasMoved = true;
            else feet[i].footHasMoved = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        int feetStepping = 0;
        int feetMoved = 0;
        foreach (Powers_StickyFoot foot in feet)
        {
            if (foot.isAnimating) feetStepping++;
            if (foot.footHasMoved) feetMoved++;
        }
        foreach (Powers_StickyFoot foot in feet)
        {
            if (feetStepping < 4) foot.TryToStep();
        }
    }

    private void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 velocity = transform.forward * v;
        pawn.SimpleMove(velocity * 9);

        transform.Rotate(0, h * 135 * Time.deltaTime, 0);

        Vector3 localVelocity = groundRing.InverseTransformDirection(velocity);
        groundRing.localPosition = Powers_AnimMath.Slide(groundRing.localPosition, localVelocity * 2, 0.001f);
    }
}
