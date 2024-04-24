using UnityEngine;

public class Enemies : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f; // Speed at which the enemy moves towards the player

    private Animator anim;
    private Transform player; // Reference to the player's transform
    private Rigidbody2D rb; // Reference to the Rigidbody2D component
    private bool playerDetected = false; // Flag to track if the player is detected
    private bool facingRight = true; // Flag to track the enemy's facing direction
    private bool moving = false; // Flag to track if the enemy is moving

    [SerializeField] private GameObject detectionRadiusObject; // Reference to the GameObject representing the detection radius

    private CircleCollider2D detectionCollider; // Reference to the CircleCollider2D for detection

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // Find the player's transform
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        anim = GetComponent<Animator>(); // Get the Animator component

        // Get the CircleCollider2D component from the detection radius GameObject
        detectionCollider = detectionRadiusObject.GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        // Call the method to detect the player
        DetectPlayer();

        // If the player is detected and not below the enemy, move towards the player
        if (playerDetected && player.position.y > transform.position.y)
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
}
