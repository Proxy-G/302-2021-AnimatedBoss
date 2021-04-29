using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Powers_HamsterAI : MonoBehaviour
{
    public Powers_GuardianController playerScript;
    [HideInInspector]
    public Transform playerPos;
    public Vector3 walkScale = Vector3.one;
    public float stepSpeed;
    [Space(10)]
    public GameObject hurtSphere;

    private AudioSource audSource;
    [Space(10)]
    public AudioClip attackSFX;
    public GameObject attackEffect;
    [Space(10)]
    public Material invincibleMat;
    public Material canHitMat;
    [Space(10)]
    public GameObject confusionIndicator;

    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public bool reachedPlayer = false;
    [HideInInspector]
    public bool inAttack = false;
    [HideInInspector]
    public bool doDamage = false;
    [HideInInspector]
    public float attackCooldown;

    private Powers_HealthSystem healthController;
    private SkinnedMeshRenderer meshRenderer;
    private Material[] mats;

    public enum States
    {
        Idle, Walk, Attack, Dead, Hitting
    }
    public States state { get; private set; }

    private void Start()
    {
        //Set necessary variables
        inAttack = false;
        doDamage = false;
        hurtSphere.SetActive(false);
        confusionIndicator.SetActive(false);

        //Get necessary components
        agent = GetComponent<NavMeshAgent>();
        playerPos = playerScript.gameObject.GetComponent<Transform>();
        healthController = GetComponent<Powers_HealthSystem>();
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        audSource = GetComponent<AudioSource>();

        //Set necessary variables with rely on components
        healthController.invincible = true;
        mats = meshRenderer.materials;
        mats[0] = invincibleMat;
        meshRenderer.materials = mats;
    }

    // Update is called once per frame
    void Update()
    {
        //Set the hamster's state for the IK animations.
        if (healthController.health == 0) 
        {
            //Set up hamster for death
            state = States.Dead;
            agent.enabled = false;
            mats[0] = invincibleMat;
            meshRenderer.materials = mats;
            confusionIndicator.SetActive(false);
        }
        else if(attackCooldown == 0) //If the hamster is NOT dead, and is NOT resting from an attack
        {
            MoveAI(); //Move the AI to the player, if AI is not attacking

            if (reachedPlayer && playerScript.healthController.health != 0 || inAttack && playerScript.healthController.health != 0) state = States.Attack;
            else if (agent.velocity.magnitude < 0.1f) state = States.Idle;
            else if (agent.velocity.magnitude >= 0.1f) state = States.Walk;

            if (state == States.Attack) inAttack = true; //Keep the hamster from leaving the attack animation until it's complete

            if(inAttack && doDamage)
            {
                state = States.Hitting; //This is to prevent animation from continuing.
                attackCooldown = 3; //Set cooldown 

                hurtSphere.SetActive(true); //Enable the attack sphere so it can hit player
                playerScript.GetComponent<Powers_PlayerTargeting>().camOrbit.shakeIntensity = 5; //Shake the camera for awesomeness!
                
                //Play attack SFX
                audSource.pitch = Random.Range(0.95f, 1);
                audSource.PlayOneShot(attackSFX);

                //Spawn attack effects
                Instantiate(attackEffect, hurtSphere.transform.position, hurtSphere.transform.rotation);

                healthController.invincible = false; //Player can hit hamster now!
                mats[0] = canHitMat;
                meshRenderer.materials = mats;
                confusionIndicator.SetActive(true);
            }
        }
        else //Else if the hamster is cooling off from an attack...
        {
            attackCooldown = Mathf.Clamp(attackCooldown - Time.deltaTime, 0, 3); //Countdown and clamp attack cooldown
            if (attackCooldown == 0)
            {
                state = States.Idle; //Reset state
                inAttack = false; //Set in attack to false if the attack cooldown has just completed.
                reachedPlayer = false; //Resetting reached player
                doDamage = false;

                healthController.invincible = true; //Player can not longer hit hamster!
                mats[0] = invincibleMat;
                meshRenderer.materials = mats;
                confusionIndicator.SetActive(false);
            }
        }
    }

    private void MoveAI()
    {
        //Move the agent
        if(state != States.Attack) agent.destination = playerPos.position;

        //If the agent has reached the player, allow the turret to prepare attack
        if (Vector3.Distance(transform.position, playerPos.position) <= agent.stoppingDistance + 0.1f) reachedPlayer = true;
        else reachedPlayer = false;
    }
}
