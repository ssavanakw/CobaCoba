using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public GameObject attack1;
    public GameObject magicMissilePrefab; // Reference to the bullet prefab
    public Transform magicMissileSpawnPoint; // The point from where the bullet will be spawned
    public int damage;
    public LayerMask enemyLayer; // To filter only enemy layer
    public Vector2 attackOffset;
    public Vector2 attackSize;

    private PlayerMovement playerMovement;
    private PlayerInput playerInput;
    private InputAction attackAction;
    private InputAction rangeAttackAction;

    private void Awake()
    {
        attack1.SetActive(false); // Ensure the attack range is disabled at the start
        playerMovement = GetComponent<PlayerMovement>(); // Get reference to PlayerMovement script
        playerInput = GetComponent<PlayerInput>(); // Get reference to PlayerInput component

        // Find the attack action from the input actions
        attackAction = playerInput.actions["Attack"];
        attackAction.performed += OnAttackPerformed;

        rangeAttackAction = playerInput.actions["RangeAttack"]; // Assuming this is mapped to K
        rangeAttackAction.performed += OnRangeAttackPerformed;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        attackAction.performed -= OnAttackPerformed;
        rangeAttackAction.performed -= OnRangeAttackPerformed;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        Attack();
    }

    private void OnRangeAttackPerformed(InputAction.CallbackContext context)
    {
        RangeAttack();
    }

    void Attack()
    {
        animator.SetBool("isAttacking", true);
    }

    void RangeAttack()
    {
        animator.SetBool("isRangeAttack", true);
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

    void EndRangeAttack()
    {
        animator.SetBool("isRangeAttack", false); // Set isAttacking to false when the attack ends
        playerMovement.enabled = true; // Enable player movement after attack ends
    }

    void ShootMagicMissile()
    {
        // Determine the direction based on the player's facing direction
        Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        // Instantiate the missile and set its direction
        GameObject missile = Instantiate(magicMissilePrefab, magicMissileSpawnPoint.position, magicMissileSpawnPoint.rotation);
        PlayerMagicMissile missileScript = missile.GetComponent<PlayerMagicMissile>();
        missileScript.direction = direction;

        // Flip the missile based on direction
        if (direction == Vector2.left)
        {
            Vector3 missileScale = missile.transform.localScale;
            missileScale.x *= -1;
            missile.transform.localScale = missileScale;
        }
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
