using UnityEngine;

public class PlayerMagicMissile : MonoBehaviour
{
    public float speed = 10f; // Speed of the magic missile
    public float lifetime = 5f; // Lifetime of the missile before it gets destroyed
    public int damage = 1; // Damage the missile will deal to enemies
    public Vector2 direction = Vector2.right; // Direction of the missile

    public LayerMask enemyLayer; // Layer for enemies
    public LayerMask obstacleLayer; // Layer for obstacles

    private Animator animator; // Reference to the Animator component
    private bool hasHit = false; // To check if the missile has already hit something

    void Start()
    {
        // Destroy the missile after its lifetime expires
        Destroy(gameObject, lifetime);

        // Get the Animator component
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Move the missile based on its speed and direction if it hasn't hit anything
        if (!hasHit)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    // Handle collision detection
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the missile collides with an enemy
        if (((1 << collision.gameObject.layer) & enemyLayer) != 0 && !hasHit)
        {
            hasHit = true;

            // Optionally, you can get a reference to the enemy's health script and reduce its health
            EnemyHealthBar enemyHealth = collision.GetComponent<EnemyHealthBar>();
            if (enemyHealth != null)
            {
                enemyHealth.EnemyTakeDamage(damage);
            }

            // Trigger the hit animation
            animator.SetTrigger("Hit");
        }
        // Destroy the missile if it hits any other object that it should collide with
        else if (((1 << collision.gameObject.layer) & obstacleLayer) != 0 && !hasHit)
        {
            hasHit = true;

            // Trigger the hit animation
            animator.SetTrigger("Hit");
        }
    }

    // This method will be called by an animation event
    void DestroyMissile()
    {
        Destroy(gameObject);
    }
}
