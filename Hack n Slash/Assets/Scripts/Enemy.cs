using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    private bool isChasing = false;

    [Header("Target")]
    [SerializeField] private LayerMask playerLayer;

    [Header("Enemy Chase")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float chaseRange = 3f;
    [SerializeField] private float jumpPower = 10f;

    [SerializeField] private Vector2 chaseSize = new Vector2(5f, 5f);

    [Header("Enemy Attack")]
    [SerializeField] private float[] attackDamages; // Array to store damage amounts for each attack
    [SerializeField] private float[] attackCooldowns; // Array to store cooldowns for each attack
    [SerializeField] private float[] attackRanges; // Array to store attack ranges for each attack
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private string[] attackAnimationTriggers;
    [SerializeField] private Collider2D[] attackColliders; // Colliders for each attack animation

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerHealth = FindObjectOfType<PlayerHealthBar>();

        // Initialize the nextAttackTimes array
        nextAttackTimes = new float[attackCooldowns.Length];
    }

    void Update()
    {
        cooldownTimer += Time.deltaTime;

        // Check if the player is within attack range
        if (PlayerInRange(attackRanges))
        {
            // Stop moving if the player is within attack range
            StopChasing();

            // Attack if cooldown allows and the enemy is not already attacking
            if (!isAttacking && !isAttackCooldown)
            {
                int availableAttack = SelectAttack();
                if (availableAttack != -1)
                {
                    Attack(availableAttack);
                    nextAttackTimes[availableAttack] = cooldownTimer + attackCooldowns[availableAttack];
                    isAttackCooldown = true;
                    StartCoroutine(ResetAttackCooldown(availableAttack));
                }
            }
        }
        else if (PlayerInRange(chaseRange))
        {
            // Continue chasing if the player is within chase range
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            // If player is neither in attack nor chase range, stop moving
            StopChasing();
            isChasing = false;
        }

        UpdateAnimation();
    }

    private int SelectAttack()
    {
        for (int i = 0; i < attackAnimationTriggers.Length; i++)
        {
            if (cooldownTimer >= nextAttackTimes[i])
            {
                return i;
            }
        }
        return -1;
    }

    private IEnumerator ResetAttackCooldown(int index)
    {
        yield return new WaitForSeconds(attackCooldowns[index]);
        isAttackCooldown = false;
    }

    private bool PlayerInRange(float[] ranges)
    {
        foreach (float range in ranges)
        {
            Collider2D hitCollider = Physics2D.OverlapBox(transform.position, new Vector2(range * 3 / 2, range * 3 / 2), 0f, playerLayer);
            if (hitCollider != null)
            {
                return true;
            }
        }
        return false;
    }

    private bool PlayerInRange(float range)
    {
        Collider2D hitCollider = Physics2D.OverlapBox(transform.position, new Vector2(range * 3 / 2, range * 3 / 2), 0f, playerLayer);
        return hitCollider != null;
    }

    private void ChasePlayer()
    {
        Vector2 direction = (playerHealth.transform.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * chaseSpeed, rb.velocity.y);

        if ((direction.x < 0 && isFacingRight) || (direction.x > 0 && !isFacingRight))
        {
            Flip();
        }

        // Check if the enemy is grounded
        bool isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, playerLayer);

        // Jump towards the player if the enemy is grounded and the player's y position is higher
        if (isGrounded && direction.y > 0.1f)
        {
            // Apply jump force
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset y velocity to avoid double jumping
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }


    private void StopChasing()
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private void Attack(int index)
    {
        isAttacking = true;
        anim.SetTrigger(attackAnimationTriggers[index]);
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
    }

    public void AttackPlayerFromAnimationEvent()
    {
        if (PlayerInRange(attackRanges) && playerHealth != null)
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
        anim.SetBool("moving", Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.y) > 0.1f);
        anim.SetBool("chasing", isChasing);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw wire cube for chase range
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(chaseSize.x, chaseSize.y, 0f));

        // Draw wire cube for attack range
        Gizmos.color = Color.blue;
        foreach (float range in attackRanges)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(range * 3 / 2, range * 3 / 2, 0f));
        }
    }
}
