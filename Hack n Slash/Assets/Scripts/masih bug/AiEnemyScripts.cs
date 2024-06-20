using System.Collections;
using UnityEngine;

public class AiEnemyScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    bool isFacingRight = true;
    public ParticleSystem dustFX;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;
    bool isIdle = false;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 1;
    int jumpsRemaining;

    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.03f);
    public LayerMask groundLayer;
    bool isGrounded = true;

    [Header("FrontGroundCheck")]
    public Transform frontGroundCheckPos;
    public Vector2 frontGroundCheckSize = new Vector2(0.5f, 0.03f);
    bool isFrontGrounded = true;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.03f);
    public LayerMask wallLayer;
    bool isNearWall = false;

    [Header("PlayerDetect")]
    public Transform playerDetectPos;
    public Vector2 playerDetectSize = new Vector2(2f, 2f); // Change detection size to a rectangle
    public float chasePlayerSpeed;
    public float stopChaseDistance = 1.5f; // Distance to stop chasing the player
    public LayerMask playerLayer;
    bool isPlayerDetected = false;
    Transform player;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpsRemaining = maxJumps;
        StartCoroutine(ChangeState());
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0f, groundLayer);
        isNearWall = Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0f, wallLayer);
        isFrontGrounded = Physics2D.OverlapBox(frontGroundCheckPos.position, frontGroundCheckSize, 0f, groundLayer);

        // Check if player is detected
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            Vector3 playerPosition = player.position;
            Vector3 enemyPosition = transform.position;

            // Check if the player is not directly below the enemy
            if (playerPosition.y < enemyPosition.y)
            {
                isPlayerDetected = false;
            }
            else
            {
                // Check box overlap to detect player
                isPlayerDetected = Physics2D.OverlapBox(playerDetectPos.position, playerDetectSize, 0f, playerLayer);
            }
        }
        else
        {
            // Player is null, reset detection flag
            isPlayerDetected = Physics2D.OverlapBox(playerDetectPos.position, playerDetectSize, 0f, playerLayer);
        }

        if (isGrounded && jumpsRemaining < maxJumps)
        {
            jumpsRemaining = maxJumps;
        }

        if (isNearWall && isGrounded)
        {
            Flip();
        }

        if (!isFrontGrounded && isGrounded)
        {
            Jump();
        }

        if (isPlayerDetected)
        {
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player").transform;
                StartCoroutine(ChasePlayer());
            }
            else
            {
                LookAtPlayer(); // Continuously look at the player
            }
        }
        else
        {
            player = null;
            if (!isIdle)
            {
                Move();
            }
        }
    }

    void Move()
    {
        if (isPlayerDetected && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < stopChaseDistance)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }
        }

        rb.velocity = new Vector2(moveSpeed * (isFacingRight ? 1 : -1), rb.velocity.y);
    }

    void Jump()
    {
        if (jumpsRemaining > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            jumpsRemaining--;
            if (dustFX != null)
            {
                dustFX.Play();
            }
        }
    }

    void Flip()
    {
        if (!isGrounded) return; // Prevent flipping while jumping

        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void LookAtPlayer()
    {
        if (player == null) return;

        Vector3 playerPosition = player.position;
        Vector3 enemyPosition = transform.position;

        if ((playerPosition.x > enemyPosition.x && !isFacingRight) || (playerPosition.x < enemyPosition.x && isFacingRight))
        {
            Flip();
        }
    }

    IEnumerator ChangeState()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            isIdle = Random.value > 0.5f;

            if (!isIdle)
            {
                if (Random.value > 0.5f)
                {
                    Flip();
                }
            }
        }
    }

    IEnumerator ChasePlayer()
    {
        yield return new WaitForSeconds(chasePlayerSpeed); // Delay before chasing

        while (isPlayerDetected)
        {
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.position);
                if (distance >= stopChaseDistance)
                {
                    Vector2 direction = (player.position - transform.position).normalized;
                    rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

                    LookAtPlayer(); // Ensure the enemy looks at the player
                }
                else
                {
                    rb.velocity = new Vector2(0, rb.velocity.y); // Stop moving if too close
                }
            }
            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(frontGroundCheckPos.position, frontGroundCheckSize);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(playerDetectPos.position, playerDetectSize); // Draw the detection area as a wire cube
    }
}
