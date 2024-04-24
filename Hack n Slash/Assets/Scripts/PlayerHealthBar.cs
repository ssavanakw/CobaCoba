using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] public Slider healthSlider; // Reference to the slider UI component
    [SerializeField] private float maxHealth = 100f; // Maximum health of the player
    [SerializeField] private float currentHealth; // Current health of the player


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth; // Set current health to maximum health when the game starts

    }
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount; // Reduce current health by the damage amount
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Clamp current health to ensure it stays within 0 and maxHealth
        Debug.Log("Player takes " + damageAmount + " damage."); // Log the damage amount
        if (currentHealth <= 0 && healthSlider.value != 0)
        {
            healthSlider.value = 0;
            Destroy(gameObject);

        }

    }

    // public void Heal(float healAmount)
    // {
    //    currentHealth += healAmount; // Increase current health by the heal amount
    //    currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Clamp current health to ensure it stays within 0 and maxHealth

    // }

    void Update()
    {
        if (healthSlider.value != currentHealth)
        {
            healthSlider.value = currentHealth;
        }

        
    }

}
