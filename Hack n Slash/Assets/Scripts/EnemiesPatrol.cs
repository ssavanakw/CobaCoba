using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class EnemiesPatrol : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPoint;
    public float speed;
    public float idleTime = 2f; // Time to idle at each point

    private bool isWaiting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPoint = pointB.transform;
        anim.SetBool("isWalk", true);
    }

    void Update()
    {
        if (!isWaiting)
        {
            Vector2 point = currentPoint.position - transform.position;
            if (currentPoint == pointB.transform)
            {
                rb.velocity = new Vector2(speed, 0);
            }
            else
            {
                rb.velocity = new Vector2(-speed, 0);
            }

            if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
            {
                StartCoroutine(WaitAtPoint(pointA.transform));
            }
            else if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
            {
                StartCoroutine(WaitAtPoint(pointB.transform));
            }
        }
    }

    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private IEnumerator WaitAtPoint(Transform nextPoint)
    {
        isWaiting = true;
        anim.SetBool("isWalk", false); // Set idle animation
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(idleTime);

        anim.SetBool("isWalk", true); // Resume walking animation
        flip();
        currentPoint = nextPoint;
        isWaiting = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }
}
