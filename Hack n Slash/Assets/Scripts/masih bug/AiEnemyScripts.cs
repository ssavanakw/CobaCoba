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
    public Vector2 playerDetectSize = new Vector2(2f, 2f);
    public float chasePlayerSpeed;
    public float stopChaseDistance = 1.5f;
    public LayerMask playerLayer;
    bool isPlayerDetected = false;
    Transform player;

    [Header("Attack")]
    public Transform attackRangePos;
    public float attackRange = 1f;
    public int attackDamage = 10;

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

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            Vector3 playerPosition = player.position;
            Vector3 enemyPosition = transform.position;

            if (playerPosition.y < enemyPosition.y)
            {
                isPlayerDetected = false;
            }
            else
            {
                isPlayerDetected = Physics2D.OverlapBox(playerDetectPos.position, playerDetectSize, 0f, playerLayer);
            }
        }
        else
        {
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
                LookAtPlayer();
                animator.SetBool("isAttacking", true); // Start attack animation
            }
        }
        else
        {
            player = null;
            if (!isIdle)
            {
                Move();
            }
            animator.SetBool("isAttacking", false); // End attack animation when player is not detected
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
                animator.SetBool("isWalk", false); // Set isWalk to false when stopping
                return;
            }
        }

        rb.velocity = new Vector2(moveSpeed * (isFacingRight ? 1 : -1), rb.velocity.y);
        animator.SetBool("isWalk", true); // Set isWalk to true when moving
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
        if (!isGrounded) return;

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
            animator.SetBool("isWalk", !isIdle); // Update isWalk based on isIdle state
        }
    }

    IEnumerator ChasePlayer()
    {
        yield return new WaitForSeconds(chasePlayerSpeed);

        while (isPlayerDetected)
        {
            if (player != null)
            {
                float distance = Vector2.Distance(transform.position, player.position);
                if (distance >= stopChaseDistance)
                {
                    Vector2 direction = (player.position - transform.position).normalized;
                    rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

                    LookAtPlayer();
                }
                else
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
            yield return null;
        }
    }

    public void OnAttack() // Ensure this method is public
    {
        if (player == null) return;

        float distance = Vector2.Distance(attackRangePos.position, player.position);
        if (distance <= attackRange)
        {
            PlayerHealthBar playerHealth = player.GetComponent<PlayerHealthBar>();
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(attackDamage);
            }
        }
    }

    public void EndAttackAnimation() // Ensure this method is public
    {
        // This method can be used to reset any flags or states after the attack animation ends
        Debug.Log("Attack animation ended.");
        animator.SetBool("isAttacking", false); // Set isAttacking to false when attack animation ends
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
        Gizmos.DrawWireCube(playerDetectPos.position, playerDetectSize);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackRangePos.position, attackRange); // Draw attack range using game object position
    }
}
