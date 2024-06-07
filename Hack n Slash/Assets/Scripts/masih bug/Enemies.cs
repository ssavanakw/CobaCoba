using UnityEngine;

public class Enemies : MonoBehaviour
{
    [SerializeField] private GameObject attackCollider1; // Reference to the AttackCollider1 GameObject
    [SerializeField] private GameObject attackCollider2; // Reference to the AttackCollider2 GameObject
    [SerializeField] private float meleeAttackRange = 1.5f; // Distance threshold for melee attacks
    [SerializeField] private float attackCooldown = 2f; // Cooldown time between attacks

    [SerializeField] private float moveSpeed = 3f; // Speed at which the enemy moves towards the player

    private Animator anim;
    private Transform player; // Reference to the player's transform
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private bool playerDetected = false; // Flag to track if the player is detected
    private bool facingRight = true; // Flag to track the enemy's facing direction
    private bool moving = false; // Flag to track if the enemy is moving
    private bool isAttacking = false; // Flag to track if the enemy is currently attacking
    private float lastAttackTime = 0f; // Timestamp of the last attack

    [SerializeField] private GameObject detectionRadiusObject; // Reference to the GameObject representing the detection radius

    private BoxCollider2D detectionCollider; // Reference to the CircleCollider2D for detection
    private PolygonCollider2D attackCollider1Collider; // Reference to the CircleCollider2D of AttackCollider1
    private PolygonCollider2D attackCollider2Collider; // Reference to the CircleCollider2D of AttackCollider2

    [SerializeField] private float damageAmount = 10f; // Damage amount when hitting the player

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player's transform
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        anim = GetComponent<Animator>(); // Get the Animator component

        // Get the CircleCollider2D component from the detection radius GameObject
        detectionCollider = detectionRadiusObject.GetComponent<BoxCollider2D>();
        attackCollider1Collider = attackCollider1.GetComponent<PolygonCollider2D>();
        attackCollider2Collider = attackCollider2.GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        // Call the method to detect the player
        DetectPlayer();

        // If the player is detected and not below the enemy, move towards the player
        if (playerDetected && !isAttacking && player.position.y > transform.position.y)
        {
            // Calculate the direction towards the player
            Vector2 direction = (player.position - transform.position).normalized;

            // Move the enemy towards the player using Rigidbody2D velocity
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            // Set moving to true if the enemy is moving horizontally
            moving = Mathf.Abs(rb.velocity.x) > 0.1f;

            // Flip the enemy if necessary
            if (direction.x < 0 && facingRight || direction.x > 0 && !facingRight)
            {
                Flip();
            }
        }
        else
        {
            // Stop the enemy if the player is not detected or is below the enemy
            rb.velocity = new Vector2(0f, rb.velocity.y);
            moving = false;
        }

        // Check if the enemy is too close to the player for a melee attack
        if (playerDetected && !isAttacking && Vector2.Distance(transform.position, player.position) <= meleeAttackRange)
        {
            // Perform a random melee attack
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                isAttacking = true;
                lastAttackTime = Time.time;
                if (Random.value < 0.5f)
                {
                    anim.SetTrigger("meleeAttack1");
                }
                else
                {
                    anim.SetTrigger("meleeAttack2");
                }
                // Stop chasing while attacking
                rb.velocity = Vector2.zero;
            }
        }

        // Update the animator parameter "Moving" based on the moving flag
        anim.SetBool("moving", moving);
    }

    private void DetectPlayer()
    {
        // Check if the detection collider is overlapping with the player
        playerDetected = detectionCollider.IsTouchingLayers(LayerMask.GetMask("Player"));
    }

    private void Flip()
    {
        // Switch the direction the enemy is facing
        facingRight = !facingRight;

        // Flip the enemy's scale along the x-axis to change its direction
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void ActivateAttackCollider1()
    {
        attackCollider1Collider.enabled = true;
    }

    public void ActivateAttackCollider2()
    {
        attackCollider2Collider.enabled = true;
    }

    public void DeActivateAttackCollider1()
    {
        attackCollider1Collider.enabled = false;
        isAttacking = false;
    }

    public void DeActivateAttackCollider2()
    {
        attackCollider2Collider.enabled = false;
        isAttacking = false;
    }
}
