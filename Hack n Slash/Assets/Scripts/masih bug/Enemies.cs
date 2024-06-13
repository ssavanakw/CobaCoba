using UnityEngine;

public class Enemies : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [Header("Attack")]
    [SerializeField] private GameObject[] attackColliders;
    [SerializeField] private float meleeAttackRange = 1.5f;
    [SerializeField] private float attackCooldown = 2f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private GameObject detectionRadiusObject;

    private Transform player;
    private bool playerDetected = false;
    private bool facingRight = true;
    private bool isAttacking = false;
    private float lastAttackTime = 0f;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        foreach (var collider in attackColliders)
        {
            collider.SetActive(false);
        }
    }

    private void Update()
    {
        DetectPlayer();

        if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
        {
            if (playerDetected && Vector2.Distance(player.position, transform.position) <= meleeAttackRange)
            {
                Attack();
            }
            else
            {
                Move();
            }
        }
    }

    private void Move()
    {
        if (playerDetected && player.position.y > transform.position.y)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

            if (direction.x < 0 && facingRight || direction.x > 0 && !facingRight)
            {
                Flip();
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        animator.SetBool("moving", rb.velocity.magnitude > 0.1f);
    }

    private void Attack()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        rb.velocity = Vector2.zero;

        int randomIndex = Random.Range(0, attackColliders.Length);
        attackColliders[randomIndex].SetActive(true);

        animator.SetTrigger("meleeAttack" + (randomIndex + 1));
    }

    private void DetectPlayer()
    {
        playerDetected = detectionRadiusObject.GetComponent<Collider2D>().IsTouchingLayers(LayerMask.GetMask("Player"));
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    public void EndAttack()
    {
        isAttacking = false;
        foreach (var collider in attackColliders)
        {
            collider.SetActive(false);
        }
    }
}
