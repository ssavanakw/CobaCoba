using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    private Animator animator;
    private FixEnemyDarat fixEnemyDarat;

    [Header("Health Bar")]
    [SerializeField] public Slider enemyHealthBarSlider;
    [SerializeField] public Slider enemyEaseHealthBarSlider;
    public float enemyMaxHealth = 100f;
    public float enemyCurrentHealth;
    public float lerpSpeed = 0.05f;

    //[SerializeField] public Slider healthSlider; // Reference to the slider UI component
    //[SerializeField] private float maxHealth = 100f; // Maximum health of the player
    //[SerializeField] public float currentHealth; // Current health of the player

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        fixEnemyDarat = GetComponent<FixEnemyDarat>();

        enemyCurrentHealth = enemyMaxHealth; // Set current health to maximum health when the game starts
        enemyHealthBarSlider.maxValue = enemyMaxHealth; // Set the slider's max value
        enemyHealthBarSlider.value = enemyCurrentHealth; // Set the slider's current value
    }


    void Update()
    {
        // Ensure the slider value matches the current health
        if (enemyHealthBarSlider.value != enemyCurrentHealth)
        {
            enemyHealthBarSlider.value = enemyCurrentHealth;
        }
        if (enemyHealthBarSlider.value != enemyEaseHealthBarSlider.value)
        {
            enemyEaseHealthBarSlider.value = Mathf.Lerp(enemyEaseHealthBarSlider.value, enemyCurrentHealth, lerpSpeed);
        }
        if (enemyCurrentHealth <= 0)
        {
            fixEnemyDarat.enabled = false; // Disable the PlayerMovement script
            animator.SetBool("Dead", true);
        }

    }

    void Die()
    {

        Destroy(gameObject); // Destroy the player object
    }


    public void EnemyTakeDamage(float damageAmount)
    {

        animator.SetTrigger("Hit");
        enemyCurrentHealth -= damageAmount; // Reduce current health by the damage amount
        enemyCurrentHealth = Mathf.Clamp(enemyCurrentHealth, 0f, enemyMaxHealth); // Clamp current health to ensure it stays within 0 and maxHealth
        Debug.Log("Enemy takes " + damageAmount + " damage."); // Log the damage amount

    }

    //public void Heal(float healAmount)
    //{
    //    enemyCurrentHealth += healAmount; // Reduce current health by the damage amount
    //    enemyCurrentHealth = Mathf.Clamp(enemyCurrentHealth, 0f, enemyMaxHealth); // Clamp current health to ensure it stays within 0 and maxHealth
    //    Debug.Log("Enemy takes " + healAmount + " damage."); // Log the damage amount
    //}

}
