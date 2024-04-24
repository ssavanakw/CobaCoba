using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator anim;
    private PlayerHealthBar playerHealth;
    private Rigidbody2D rb;

    private float cooldownTimer = Mathf.Infinity;
    private float[] nextAttackTimes; // Array to store next attack times for each attack

    private bool isFacingRight = true;
    private bool isAttacking = false;
    private bool isAttackCooldown = false;

    [Header("Target")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Enemy Chase")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float chaseRange = 3f;
    [SerializeField] private Vector2 chaseSize = new Vector2(5f, 5f);

    [Header("Enemy Attack")]
    [SerializeField] private float[] attackDamages; // Array to store damage amounts for each attack
    [SerializeField] private float[] attackCooldowns; // Array to store cooldowns for each attack
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float attackRange;
    [SerializeField] private Vector2 attackSize = new Vector2(5f, 5f);
    [SerializeField] private string[] attackAnimationTriggers;
    [SerializeField] private Collider2D[] attackColliders; // Colliders for each attack animation

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerHealth = FindObjectOfType<PlayerHealthBar>();

        // Initialize the nextAttackTimes array
        nextAttackTimes = new float[attackCooldowns.Length];
        for (int i = 0; i < nextAttackTimes.Length; i++)
        {
            nextAttackTimes[i] = 0f;
        }
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Check if the player is within attack range
        if (PlayerInRange(attackRange))
        {
            // Stop moving if the player is within attack range
            StopChasing();

            // Attack if cooldown allows and the enemy is not already attacking
            if (!isAttacking && !isAttackCooldown)
            {
                List<int> availableAttacks = new List<int>(); // List to store available attacks

                for (int i = 0; i < attackAnimationTriggers.Length; i++)
                {
                    if (cooldownTimer >= nextAttackTimes[i])
                    {
                        availableAttacks.Add(i); // Add the index of available attack to the list
                    }
                    else
                    {
                        Debug.Log("Attack " + (i + 1) + " is on cooldown.");
                    }
                }

                // Check if there are available attacks
                if (availableAttacks.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, availableAttacks.Count); // Select a random index from available attacks
                    int selectedAttackIndex = availableAttacks[randomIndex]; // Get the selected attack index

                    Attack(attackAnimationTriggers[selectedAttackIndex], selectedAttackIndex); // Pass the selected index to Attack method
                    nextAttackTimes[selectedAttackIndex] = cooldownTimer + attackCooldowns[selectedAttackIndex];
                    isAttackCooldown = true;
                    StartCoroutine(ResetAttackCooldown(attackCooldowns[selectedAttackIndex])); // Reset cooldown based on individual cooldown time
                }
            }
        }
        else if (PlayerInRange(chaseRange))
        {
            // Continue chasing if the player is within chase range
            ChasePlayer();
        }
        else
        {
            // If player is neither in attack nor chase range, stop moving
            StopChasing();
        }

        UpdateAnimation();
    }


    private IEnumerator ResetAttackCooldown(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        isAttackCooldown = false;
    }

    private bool PlayerInRange(float range)
    {
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(range * 2, range * 2), 0f, playerLayer);
        return hitColliders.Length > 0;
    }

    private void ChasePlayer()
    {
        Vector2 direction = (playerHealth.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);

        if ((direction.x < 0 && isFacingRight) || (direction.x > 0 && !isFacingRight))
        {
            Flip();
        }
    }

    private void StopChasing()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private void Attack(string attackAnimationTrigger, int index)
    {
        isAttacking = true;
        anim.SetTrigger(attackAnimationTrigger);
        cooldownTimer = 0f;

        // Set damage amount for the corresponding attack
        if (index >= 0 && index < attackAnimationTriggers.Length && index < attackDamages.Length)
        {
            damageAmount = attackDamages[index];
        }
        else
        {
            Debug.LogError("Invalid attack animation trigger or damage index!");
        }

        // Activate the collider corresponding to the current attack animation
        for (int i = 0; i < attackAnimationTriggers.Length; i++)
        {
            if (attackAnimationTriggers[i] == attackAnimationTrigger)
            {
                attackColliders[i].enabled = true;
            }
            else
            {
                attackColliders[i].enabled = false;
            }
        }
    }

    public void AttackPlayerFromAnimationEvent()
    {
        if (PlayerInRange(attackRange) && playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }

        isAttacking = false;
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    private void UpdateAnimation()
    {
        anim.SetBool("moving", Mathf.Abs(rb.velocity.x) > 0.1f);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw wire box for chase range
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(chaseSize.x, chaseSize.y, 0f));

        // Draw wire box for attack range
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(attackSize.x, attackSize.y, 0f));
    }
}