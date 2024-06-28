using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAttack : MonoBehaviour
{
    public Vector2 meleeAttack1Size = new Vector2(3f, 3f); // Size of the box for meleeAttack1
    public Vector2 meleeAttack2Size = new Vector2(4f, 4f); // Size of the box for meleeAttack2
    public LayerMask playerLayer;
    public int meleeAttack1Damage = 10;
    public int meleeAttack2Damage = 20;
    public float attackCooldown = 2f;

    private bool canAttack = true;

    void Update()
    {
        if (canAttack)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                MeleeAttack1();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                MeleeAttack2();
            }
        }
    }

    public void MeleeAttack1()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(transform.position, meleeAttack1Size, 0f, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealthBar>().TakeDamage(meleeAttack1Damage);
        }
        StartCoroutine(AttackCooldown());
    }

    public void MeleeAttack2()
    {
        Collider2D[] hitPlayers = Physics2D.OverlapBoxAll(transform.position, meleeAttack2Size, 0f, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealthBar>().TakeDamage(meleeAttack2Damage);
        }
        StartCoroutine(AttackCooldown());
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(meleeAttack1Size.x, meleeAttack1Size.y, 0));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(meleeAttack2Size.x, meleeAttack2Size.y, 0));
    }
}
