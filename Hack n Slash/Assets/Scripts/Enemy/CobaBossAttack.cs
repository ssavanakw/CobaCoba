using UnityEngine;

public class CobaBossAttack : MonoBehaviour
{
    public GameObject projectilePrefab; // The projectile prefab to be used for ranged attacks
    public Transform firePoint;         // The point from where projectiles will be fired
    public float projectileSpeed = 10f; // Speed of the projectiles
    public float attackRange = 10f;     // The range within which the boss will attack
    public float meleeRange = 2f;       // The range within which the boss will perform melee attacks
    public float attackCooldown = 2f;   // Time between attacks

    private Transform player;
    private float attackTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        attackTimer = attackCooldown;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange && attackTimer <= 0)
            {
                if (distanceToPlayer <= meleeRange)
                {
                    PerformMeleeAttack();
                }
                else
                {
                    PerformRangedAttack();
                }

                attackTimer = attackCooldown;
            }
        }
    }

    void PerformRangedAttack()
    {
        // Instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = (player.position - firePoint.position).normalized * projectileSpeed;
    }

    void PerformMeleeAttack()
    {
        // Implement melee attack logic here
        Debug.Log("Performing melee attack!");
        // This could involve reducing the player's health, playing an animation, etc.
    }

    private void OnDrawGizmosSelected()
    {
        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Draw melee range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
