using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] public Slider healthSlider; // Reference to the slider UI component
    [SerializeField] private float maxHealth = 100f; // Maximum health of the player
    [SerializeField] public float currentHealth; // Current health of the player

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; // Set current health to maximum health when the game starts
        healthSlider.maxValue = maxHealth; // Set the slider's max value
        healthSlider.value = currentHealth; // Set the slider's current value
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount; // Reduce current health by the damage amount
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Clamp current health to ensure it stays within 0 and maxHealth
        Debug.Log("Player takes " + damageAmount + " damage."); // Log the damage amount
        healthSlider.value = currentHealth; // Update the slider's current value

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Player died."); // Log the player's death
        Destroy(gameObject); // Destroy the player object
    }

    void Update()
    {
        // Ensure the slider value matches the current health
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }
    }
}
