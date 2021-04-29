using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_HurtSphere : MonoBehaviour
{
    private SphereCollider hurtSphere;
    private float sphereDisableCountdown = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        hurtSphere = GetComponent<SphereCollider>(); //Get the collider
    }

    // Update is called once per frame
    void OnEnable()
    {
        sphereDisableCountdown = 0.1f; //On the object's enabling, reset the countdown to disable the object.
    }

    private void Update()
    {
        sphereDisableCountdown -= Time.deltaTime; //Countdown

        if(sphereDisableCountdown < 0) gameObject.SetActive(false); //Once countdown is complete, disable the object.
    }

    private void OnTriggerEnter(Collider other)
    {
        Powers_GuardianController playerScript = other.GetComponent<Powers_GuardianController>();

        if(playerScript) //If there is a valid player script...
        {
            playerScript.GetComponent<Powers_HealthSystem>().TakeDamage(Random.Range(39, 45)); //Damage the player!

            gameObject.SetActive(false); //Disable the hurt sphere
        }
    }
}
