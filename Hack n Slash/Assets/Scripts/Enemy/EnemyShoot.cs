using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    public Transform turretHead; // Added this line
    public float shootDistance = 6f;
    public float shootInterval = 1f;

    private float timer;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < shootDistance)
        {
            // Rotate turret head to face the player
            Vector2 direction = player.transform.position - turretHead.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turretHead.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            timer += Time.deltaTime;

            if (timer > shootInterval)
            {
                timer = 0;
                shoot();
            }
        }
    }

    void shoot()
    {
        Instantiate(bullet, bulletPos.position, turretHead.rotation); // Adjusted this line to use the turret head's rotation
    }

    // This method is called by Unity to draw Gizmos in the Scene view
    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position with the radius of shootingDistance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootDistance);
    }
}
