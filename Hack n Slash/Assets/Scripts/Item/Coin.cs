using UnityEngine;
using System.Collections; // Add this line

public class Coin : MonoBehaviour
{
    public int coinValue = 1;
    public Animator animator; // Reference to the Animator component
    private bool isCollected = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            isCollected = true; // Prevent multiple triggers
            PlayerCollecting player = other.GetComponent<PlayerCollecting>();
            if (player != null)
            {
                animator.SetTrigger("Hit");
                player.AddCoin(coinValue);
            }

            // Destroy the coin game object after the animation
            StartCoroutine(DestroyAfterAnimation());
        }
    }

    private IEnumerator DestroyAfterAnimation()
    {
        // Wait until the end of the current animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
}
