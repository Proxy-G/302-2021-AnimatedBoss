using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers_HealthSystem : MonoBehaviour
{
    [HideInInspector]
    public float health;
    public float maxHealth = 100;
    public float regenSpeed = 0;
    public bool invincible = false;
    public AudioClip hurtSFX;

    private AudioSource audSource;

    private void Start()
    {
        health = maxHealth;
        audSource = GetComponent<AudioSource>(); //Get audio source component
    }

    public void Update()
    {
        if(health > 0) health += regenSpeed * Time.deltaTime;
        health = Mathf.Clamp(health, 0, maxHealth);
    }

    public void TakeDamage(float damage){
        
        if(!invincible) //If the target is invincible currently, ignore!
        {
            if (damage <= 0) return; //Return if damage float is less than 0 or equal to 0

            health -= damage; //Hurt the object

            if (audSource && hurtSFX)
            {
                audSource.pitch = Random.Range(0.95f, 1); //Randomize pitch for hurt SFX playback
                audSource.PlayOneShot(hurtSFX); //Play hurt SFX
            }
        }
    }
}
