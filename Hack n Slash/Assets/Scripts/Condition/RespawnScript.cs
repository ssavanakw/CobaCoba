using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnScript : MonoBehaviour
{

    public GameObject player;
    public GameObject respawnPoint;
    public float damage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            player.transform.position = respawnPoint.transform.position;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealthBar playerHealth = other.gameObject.GetComponent<PlayerHealthBar>();
            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(damage); // Use the TakeDamage method
                
            }
            other.gameObject.GetComponent<PlayerHealthBar>().currentHealth -= 10;
            player.transform.position = respawnPoint.transform.position;
        }
    }


}
