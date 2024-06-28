using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeath : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        // Check if the collision is with the player
        if (other.gameObject.CompareTag("Player"))
        {
            // Get the player's health component
            PlayerHealthBar playerHealth = other.gameObject.GetComponent<PlayerHealthBar>();

            // If the player's health component exists, reduce their health to zero
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(playerHealth.currentHealth);
            }
        }
    }

}
