using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDeath : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collision is with the player
        if (other.gameObject.CompareTag("Player"))
        {
            // Get the player's health component
            PlayerHealthBar playerHealthBar = other.gameObject.GetComponent<PlayerHealthBar>();

            // If the player's health component exists, reduce their health to zero
            if (playerHealthBar != null)
            {
                Debug.Log("Player health component found. Setting health to zero.");
                playerHealthBar.PlayerTakeDamage(playerHealthBar.currentHealth);
            }
        }
    }

}
