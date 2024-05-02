using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public int damage = 10; // Damage inflicted by this attack collider
    public float cooldown = 1f; // Cooldown time for this attack

    private bool canAttack = true; // Flag to track if the collider can currently attack

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider can attack and the collided object is the player
        if (canAttack && other.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerHealthBar playerHealth = other.GetComponent<PlayerHealthBar>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // Start cooldown
            canAttack = false;
            Invoke("ResetCooldown", cooldown);
        }
    }

    private void ResetCooldown()
    {
        canAttack = true;
    }
}
