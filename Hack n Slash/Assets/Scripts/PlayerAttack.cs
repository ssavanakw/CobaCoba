using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public GameObject attack1;
    public int damage;
    public LayerMask enemyLayer;  // To filter only enemy layer
    public Vector2 attackOffset;
    public Vector2 attackSize;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        attack1.SetActive(false);  // Ensure the attack range is disabled at the start
        playerMovement = GetComponent<PlayerMovement>(); // Get reference to PlayerMovement script
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    void Attack()
    {
        animator.SetBool("isAttacking", true);
    }

    void StopMove()
    {
        playerMovement.enabled = false; // Disable player movement during attack
    }


    // This method will be called from the animation event
    void EnableAttack1()
    {
        attack1.SetActive(true);
        CheckForEnemyHit();
    }

    // This method will be called from the animation event
    void DisableAttack1()
    {
        attack1.SetActive(false);
    }

    void EndAttack()
    {
        animator.SetBool("isAttacking", false); // Set isAttacking to false when the attack ends
        playerMovement.enabled = true; // Enable player movement after attack ends
    }

    // Method to check if enemy is within the attack range
    void CheckForEnemyHit()
    {
        Vector2 attackPosition = (Vector2)attack1.transform.position + attackOffset;
        Collider2D hitEnemy = Physics2D.OverlapBox(attackPosition, attackSize, 0, enemyLayer);

        if (hitEnemy != null)
        {
            EnemyHealthBar enemyHealthBar = hitEnemy.GetComponent<EnemyHealthBar>();
            if (enemyHealthBar != null)
            {
                enemyHealthBar.EnemyTakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the attack range in the editor
        Gizmos.color = Color.red;
        Vector3 attackPosition = attack1.transform.position + (Vector3)attackOffset;
        Gizmos.DrawWireCube(attackPosition, attackSize);
    }
}
