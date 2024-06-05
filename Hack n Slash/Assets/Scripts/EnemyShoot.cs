using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletPos;
    public float shootDistance = 6f;

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
        Debug.Log(distance);

        if (distance < shootDistance)
        {
            timer += Time.deltaTime;

            if (timer > 1)
            {
                timer = 0;
                shoot();
            }

        }


    }

    void shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }

    // This method is called by Unity to draw Gizmos in the Scene view
    private void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position with the radius of shootingDistance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootDistance);
    }



}
