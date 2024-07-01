using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPlayer : MonoBehaviour
{
    public PlayerHealthBar playerHealth;

    // Amount to heal the player
    public float healAmount;

    // This method is called when another collider enters the trigger collider attached to this GameObject
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object we collided with has the Player tag
        if (other.CompareTag("Player"))
        {
            // Get the player's health script
            PlayerHealthBar playerHealth = other.GetComponent<PlayerHealthBar>();
            if (playerHealth != null)
            {
                // Heal the player
                playerHealth.PlayerHeal(healAmount);

                // Optionally, destroy this healing object after use
                Destroy(gameObject);
            }
        }
    }
}
