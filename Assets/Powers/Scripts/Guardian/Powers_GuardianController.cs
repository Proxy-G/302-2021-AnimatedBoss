using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_GuardianController : MonoBehaviour
{
    public enum States
    {
        Idle, Walk, Run, Jump, Dead
    }

    private CharacterController pawn;
    private Camera cam;
    private Powers_CamOrbit camOrbit;
    [HideInInspector]
    public Powers_HealthSystem healthController;
    public float walkSpeed = 5;
    public float runSpeed = 10;
    public float gravityMultiplier = 10;
    public float jumpImpulse = 5;
    [Space(10)]
    public float stepWalkSpeed = 5;
    public float stepRunSpeed = 10;

    public Vector3 walkScale = Vector3.one;
    public Vector3 runScale = Vector3.one;
    public AnimationCurve ankleWalkRotationCurve;
    [Space(10)]
    public GameObject charRoot;

    [HideInInspector]
    public float moveSpeed;
    [HideInInspector]
    public float stepSpeed;

    public States state {get; private set;}
    [HideInInspector]
    public Vector3 moveDir;
    [HideInInspector]
    public Vector3 moveDirAnim;

    private float timeLeftGrounded = 0;
    private bool jumpSFXplayed = false;

    [Space(10)]
    private AudioSource audSource;
    public List<AudioClip> jumpSFX = new List<AudioClip>();
    public AudioClip deathSFX;
    private bool deathSFXplayed = false;

    public bool isGrounded
    {
        get
        { //return true if pawn is on ground OR "coyote-time" is not zero
            return pawn.isGrounded || timeLeftGrounded > 0;
        }
    }

    /// <summary>
    /// How fast player is currently moving vertically (y-axis), in meters/sec.
    /// </summary>
    private float verticalVelocity = 0;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = walkSpeed;
        stepSpeed = stepWalkSpeed;
        
        pawn = GetComponent<CharacterController>();
        healthController = GetComponent<Powers_HealthSystem>();
        audSource = GetComponent<AudioSource>();

        cam = Camera.main;
        camOrbit = cam.GetComponentInParent<Powers_CamOrbit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (healthController.health != 0) //If the player is not dead...
        {
            HorizontalMovement(); //Determine movement on the x and z axis:
            VerticalMovement(); //Determine movement on the y axis:

            //adds lateral movement to vertical movement:
            Vector3 moveDelta = moveDir * moveSpeed + verticalVelocity * Vector3.down;

            //move pawn:
            CollisionFlags flags = pawn.Move(moveDelta * Time.deltaTime);

            moveDirAnim = transform.InverseTransformDirection(moveDir);
        }
        else
        {
            state = States.Dead; //set the player to be in dead state
            Death();
        }
    }

    //This function is used to get the movement on the x and z axis.
    void HorizontalMovement()
    {
        //Get the x and z axis movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveDir = transform.forward * v + transform.right * h;
        if (moveDir.sqrMagnitude > 1) moveDir.Normalize();

        //Determine the controller's current state
        if (!pawn.isGrounded) state = States.Jump;
        else if (moveDir.magnitude < .1f || pawn.velocity.magnitude < 0.25f) state = States.Idle;
        else state = (Input.GetButton("Shift") && moveDirAnim.z > 0.15f && !camOrbit.IsTargeting()) ? States.Run : States.Walk;

        //Decide movement and step speed based on controller's current state
        switch (state)
        {
            case States.Idle:
                moveSpeed = walkSpeed;
                stepSpeed = stepWalkSpeed;
                break;
            case States.Walk:
                moveSpeed = walkSpeed;
                stepSpeed = stepWalkSpeed;
                break;
            case States.Run:
                moveSpeed = runSpeed;
                stepSpeed = stepRunSpeed;
                break;
        }

        bool isTryingToMove = (h != 0 || v != 0);
        if (isTryingToMove)
        {
            //Turn to face correct direction
            float camYaw = cam.transform.eulerAngles.y;
            transform.rotation = Powers_AnimMath.Slide(transform.rotation, Quaternion.Euler(0, camYaw, 0), .01f);
        }
    }
    
    //This function is used to get the movement on the y axis.
    void VerticalMovement()
    {

        if (pawn.isGrounded)
        {
            moveDir.y = 0; //on ground, zero-out vertical-velocity
            timeLeftGrounded = .1f;
            jumpSFXplayed = false;
        }

        //Coyote time countdown:
        if (timeLeftGrounded > 0) timeLeftGrounded -= Time.deltaTime;

        if (isGrounded) //If the player is grounded
        {
            if (Input.GetButtonDown("Jump"))
            {
                //Jump!
                verticalVelocity = -jumpImpulse;
                //Play random SFX from jump SFX list.
                if (!jumpSFXplayed) audSource.PlayOneShot(jumpSFX[Random.Range(0, jumpSFX.Count)]);
                jumpSFXplayed = true;
            }
            //else verticalVelocity = 10; //To prevent character controller bouncing when walking down slopes
        }
        else verticalVelocity += gravityMultiplier * Time.deltaTime; //Apply gravity:

    }

    void Death()
    {
        if(!deathSFXplayed)
        {
            //Disable char controller and scripts affecting animation
            pawn.enabled = false;
            GetComponent<Powers_PlayerTargeting>().enabled = false;
            GetComponentInChildren<Animator>().enabled = false;
            camOrbit.isDead = true;

            deathSFXplayed = true; //Death setup is complete
        }

        charRoot.transform.localPosition = Powers_AnimMath.Slide(charRoot.transform.localPosition, new Vector3(0, 0.3f, 0), 0.01f);
        charRoot.transform.localEulerAngles = Powers_AnimMath.Slide(charRoot.transform.localEulerAngles, new Vector3(90, 0, 0), 0.01f);
    }
}
