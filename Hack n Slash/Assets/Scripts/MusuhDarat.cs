using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MusuhDarat : Enemy
{
    // Variables
    private enum State { idle, running, attack1, attack2 };
    private State state = State.idle;
    private float attackTimer1 = 2f;
    private AudioSource audioSource;
    [SerializeField] private float attackTimer1Set = 1.8f;
    [SerializeField] private Transform attackPoint1;
    [SerializeField] private float attackRange1 = 0.5f; // Default range value
    [SerializeField] private int attackDamage1 = 10; // Default damage value
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private AIPath path;
    private Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        path = GetComponent<AIPath>();
    }

    void Update()
    {
        if (path.desiredVelocity.magnitude >= 0.1f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }

        attackTimer1 -= Time.deltaTime;
        if (attackTimer1 <= 0f)
        {
            Attack1();
            attackTimer1 = attackTimer1Set;
        }

        UpdateAnimator();
    }

    private void Attack1()
    {
        state = State.attack1;
        animator.SetInteger("state", 2); // Set animator state to attack1
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint1 == null) return;

        Gizmos.DrawWireSphere(attackPoint1.position, attackRange1);
    }

    // This function will be called from the animation event
    public void OnAttack1AnimationEvent()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint1.position, attackRange1, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealthBar>().PlayerTakeDamage(attackDamage1);
        }
    }

    private void UpdateAnimator()
    {
        if (state == State.running)
        {
            animator.SetInteger("state", 1); // Set animator state to running
        }
        else if (state == State.idle)
        {
            animator.SetInteger("state", 0); // Set animator state to idle
        }
        // Add other state transitions if needed
    }
}
