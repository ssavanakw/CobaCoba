using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement playerMovement;

    [Header("Health Bar")]
    [SerializeField] public Slider healthBarSlider;
    [SerializeField] public Slider easeHealthBarSlider;
    public float maxHealth = 100f;
    public float currentHealth;
    public float lerpSpeed = 0.05f;

    //[SerializeField] public Slider healthSlider; // Reference to the slider UI component
    //[SerializeField] private float maxHealth = 100f; // Maximum health of the player
    //[SerializeField] public float currentHealth; // Current health of the player

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        playerMovement.enabled = true; // Disable the PlayerMovement script
        currentHealth = maxHealth; // Set current health to maximum health when the game starts
        healthBarSlider.maxValue = maxHealth; // Set the slider's max value
        healthBarSlider.value = currentHealth; // Set the slider's current value
    }


    void Update()
    {
        // Ensure the slider value matches the current health
        if (healthBarSlider.value != currentHealth)
        {
            healthBarSlider.value = currentHealth;
        }
        if (healthBarSlider.value != easeHealthBarSlider.value)
        {
            easeHealthBarSlider.value = Mathf.Lerp(easeHealthBarSlider.value, currentHealth, lerpSpeed);
        }
        if (currentHealth <= 0)
        {
            playerMovement.enabled = false; // Disable the PlayerMovement script
            animator.SetBool("isDead", true);
        }
    }
    void Die()
    {
        Destroy(gameObject); // Destroy the player object
    }


    public void PlayerTakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount; // Reduce current health by the damage amount
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Clamp current health to ensure it stays within 0 and maxHealth
        Debug.Log("Player takes " + damageAmount + " damage."); // Log the damage amount


    }
    public void PlayerHeal(float healAmount)
    {
        currentHealth += healAmount; // Reduce current health by the damage amount
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Clamp current health to ensure it stays within 0 and maxHealth
        Debug.Log("Player takes " + healAmount + " damage."); // Log the damage amount

    }

}
