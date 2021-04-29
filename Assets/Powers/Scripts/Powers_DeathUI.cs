using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Powers_DeathUI : MonoBehaviour
{
    public Powers_HealthSystem playerHealth;
    public CanvasGroup playerHealthbarHolder;
    public CanvasGroup bossHealthbarHolder;
    public CanvasGroup deathUI;

    public float retryAllowedCountdown = 0.5f;

    void Start()
    {
        playerHealthbarHolder.alpha = 1;
        playerHealthbarHolder.interactable = false;
        playerHealthbarHolder.blocksRaycasts = false;

        bossHealthbarHolder.alpha = 1;
        bossHealthbarHolder.interactable = false;
        bossHealthbarHolder.blocksRaycasts = false;

        deathUI.alpha = 0;
        deathUI.interactable = false;
        deathUI.blocksRaycasts = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHealth.health == 0)
        {
            retryAllowedCountdown = Mathf.Clamp(retryAllowedCountdown -= Time.deltaTime, 0, 0.1f); //Countdown and clamp until retry allowed
            
            Cursor.lockState = CursorLockMode.None;
            playerHealthbarHolder.alpha = Powers_AnimMath.Slide(playerHealthbarHolder.alpha, 0, 0.02f);
            bossHealthbarHolder.alpha = Powers_AnimMath.Slide(bossHealthbarHolder.alpha, 0, 0.02f);
            deathUI.alpha = Powers_AnimMath.Slide(deathUI.alpha, 1, 0.02f);

            deathUI.interactable = true;
            deathUI.blocksRaycasts = true;
        }
    }
}
