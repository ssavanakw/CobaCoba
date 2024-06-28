using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    public int damage; // Damage inflicted by this attack collider

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is the player
        if (other.CompareTag("Player"))
        {
            // Deal damage to the player
            PlayerHealthBar playerHealth = other.GetComponent<PlayerHealthBar>();
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(damage);
            }
        }
    }
}
