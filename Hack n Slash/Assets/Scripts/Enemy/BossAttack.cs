using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BossAttack : MonoBehaviour
{
    public Animator animator;
    public GameObject bossAttack1;  // Assuming enemyAttack1 is the attack range GameObject

    public LayerMask playerLayer;  // To filter only player layer
    bool canAttack = true;
    public float attackCooldown = 2f;

    [Header("Attack 1")]
    public int damage;
    public Vector2 attackOffset;
    public Vector2 attackSize;


    private void Awake()
    {
        bossAttack1.SetActive(false);  // Ensure the attack range is disabled at the start

    }

    private void Update()
    {

    }

    // This method will be called from the animation event
    void EnableAttack1()
    {
        bossAttack1.SetActive(true);
        CheckForPlayerHit();
    }

    // This method will be called from the animation event
    void DisableAttack1()
    {
        bossAttack1.SetActive(false);
        animator.SetBool("Attack", false); // Set isAttacking to false when the attack ends
    }



    // Method to check if player is within the attack range
    void CheckForPlayerHit()
    {
        Vector2 attackPosition = (Vector2)transform.position + attackOffset;
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(attackPosition, attackSize, 0, playerLayer);

        foreach (Collider2D hitPlayer in hitPlayers)
        {
            PlayerHealthBar playerHealthBar = hitPlayer.GetComponent<PlayerHealthBar>();
            if (playerHealthBar != null)
            {
                playerHealthBar.PlayerTakeDamage(damage);
            }
        }
    }

    public void MeleeAttack1()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(transform.position, attackSize, 0f, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealthBar>().PlayerTakeDamage(damage);
        }
        StartCoroutine(AttackCooldown());
    }

    //public void MeleeAttack2()
    //{
    //    Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(transform.position, attack2Size, 0f, playerLayer);
    //    foreach (Collider2D player in hitPlayers)
    //    {
    //        player.GetComponent<PlayerHealthBar>().PlayerTakeDamage(damage2);
    //    }
    //    StartCoroutine(AttackCooldown());
    //}

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        // Visualize the attack range in the editor
        Gizmos.color = Color.red;
        Vector3 attackPosition = bossAttack1.transform.position + (Vector3)attackOffset;
        Gizmos.DrawWireCube(attackPosition, attackSize);
    }

}
