using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    bool isFacingRight = true;
    public ParticleSystem dustFX;

    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    [Header("GroundCheck")]
    [SerializeField] private LayerMask groundLayer;
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2 (0.5f, 0.03f);
    
    bool isGrounded = true;

    [Header("Gravity")]
    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    [Header("WallCheck")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.03f);
    public LayerMask wallLayer;

    [Header("WallMovement")]
    //Wall slide
    public float wallSlideSpeed = 2;
    bool isWallSliding;

    // Wall jump
    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2 (5f, 10f);

    void Update()
    {
        
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        WallJump();
        

        if(!isWallJumping)
        {
            rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
            Flip();
        }


        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetFloat("magnitude", rb.velocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0)
        {
            animator.SetBool("isAttacking", false);
            animator.SetBool("isRangeAttack", false);

            // Hold down jump for full height
            if (context.performed)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                jumpsRemaining--;
                JumpFX();
            }
            // Light tap jump for half the height
            else if (context.canceled && rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                jumpsRemaining--;
                JumpFX();
            }
        }

        //Wall jump
        if(context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            //Jump Away
            wallJumpTimer = 0f;
            JumpFX();

            //Force Flip
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 Is = transform.localScale;
                Is.x *= -1f;
                transform.localScale = Is;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);

        }
        
    }

    private void JumpFX()
    {
        animator.SetTrigger("jump");
        dustFX.Play();
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;

            // Tambahan ini
            isWallJumping = false;
        }
        else
        {
            isGrounded = false;
        }

    }

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    public void ProcessGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if(!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }
    private void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement >0)
        {
            isFacingRight = !isFacingRight;
            Vector3 Is = transform.localScale;
            Is.x *= -1f;
            transform.localScale = Is;

            if (rb.velocity.y == 0f)
            {
                dustFX.Play();
            }


        }
    }

    private void OnDrawGizmosSelected()
    {
        // GroundCheck
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        // WallCheck
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }


}
