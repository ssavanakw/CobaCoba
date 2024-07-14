using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{
    public int healAmount = 20;  // Amount of health to restore
    public GameObject maxHealthText;  // Reference to the UI text object for max health message
    public float maxHealthTextDuration = 1f;  // Duration to keep the max health text active
    public Animator animatorHeal;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealthBar playerHealth = collision.GetComponent<PlayerHealthBar>();
        if (playerHealth != null)
        {
            if (playerHealth.currentHealth < playerHealth.maxHealth)
            {
                playerHealth.PlayerHeal(healAmount);
                animatorHeal.SetTrigger("Hit");
                // Destroy the healing object after use
                //Destroy(gameObject);
            }
            else
            {
                if (maxHealthText != null)
                {
                    StartCoroutine(ShowMaxHealthText());
                }
            }
        }
    }

    void DestroyThisHeal()
    {
        Destroy(gameObject);
    }

    private IEnumerator ShowMaxHealthText()
    {
        maxHealthText.SetActive(true);
        yield return new WaitForSeconds(maxHealthTextDuration);
        maxHealthText.SetActive(false);
    }
}
