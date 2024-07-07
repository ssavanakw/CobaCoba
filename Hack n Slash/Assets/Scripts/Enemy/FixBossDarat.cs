using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEditor.Tilemaps;
using UnityEngine.UI;
using UnityEngine;

public class FixBossDarat : MonoBehaviour
{

    #region Public Variables
    public Transform rayCast;
    public LayerMask raycastMask;
    public float rayCastLength;
    public float attackDistance;
    public float moveSpeed;
    public float timer;
    public Transform pointA;
    public Transform pointB;
    public GameObject enemyHealthBar;
    #endregion

    #region Private Variables
    private RaycastHit2D hit;
    private Transform target;
    private Animator anim;
    private float distance;
    private bool attackMode;
    private bool inRange;
    private bool cooling;
    private float intTimer;
    #endregion


    // Start is called before the first frame update
    void Awake()
    {
        SelectTarget();
        intTimer = timer;
        anim = GetComponent<Animator>();

        enemyHealthBar.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (!attackMode)
        {
            BossMove();
        }

        if (!InsideOfPoints() && !inRange & !anim.GetCurrentAnimatorStateInfo(0).IsName("HG_Attack"))
        {
            SelectTarget();
        }


        if (inRange)
        {
            hit = Physics2D.Raycast(rayCast.position, transform.right, rayCastLength, raycastMask);
            RaycastDebugger();
        }

        // Player Detected

        if (hit.collider != null)
        {
            BossEnemyLogic();
        }
        else if (hit.collider == null)
        {
            inRange = false;
        }

        if (inRange == false)
        {
            BossStopAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "Player")
        {
            target = trig.transform;
            inRange = true;
            enemyHealthBar.SetActive(true);
            BossFlip();
        }
    }

    void BossEnemyLogic()
    {

        distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackDistance)
        {
            BossStopAttack();
        }
        else if (attackDistance >= distance && cooling == false)
        {
            BossAttack();
        }

        if (cooling)
        {
            BossCooldown();
            anim.SetBool("Attack", false);
        }
    }

    void BossMove()
    {
        anim.SetBool("canWalk", true);
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("BOD_Attack"))
        {
            Vector2 targetPosition = new Vector2(target.position.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        }
    }

    void BossAttack()
    {
        timer = intTimer;
        attackMode = true;

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);

    }

    void BossCooldown()
    {
        timer -= Time.deltaTime;
        Debug.Log("Cooldown Timer: " + timer.ToString("F2") + " seconds");

        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }


    void BossStopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }


    void RaycastDebugger()
    {
        if (distance > attackDistance)
        {
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.red);
        }
        else if (attackDistance > distance)
        {
            Debug.DrawRay(rayCast.position, transform.right * rayCastLength, Color.green);
        }

    }

    public void BossTriggerCooling()
    {
        cooling = true;
    }

    private bool InsideOfPoints()
    {
        return transform.position.x > pointA.position.x && transform.position.x < pointB.position.x;
    }

    private void SelectTarget()
    {
        float distanceToLeft = Vector2.Distance(transform.position, pointA.position);
        float distanceToRight = Vector2.Distance(transform.position, pointB.position);
        if (distanceToLeft > distanceToRight)
        {
            target = pointA;
        }
        else
        {
            target = pointB;
        }

        BossFlip();

    }

    private void BossFlip()
    {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x < target.position.x)
        {
            rotation.y = 180f;
        }
        else
        {
            rotation.y = 0f;
        }

        transform.eulerAngles = rotation;

    }


}
