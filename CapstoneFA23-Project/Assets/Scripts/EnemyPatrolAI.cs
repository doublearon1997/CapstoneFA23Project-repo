using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyPatrolAI : MonoBehaviour
{
    public GameObject pointA;
    public GameObject pointB;
    private Rigidbody2D rb;
    private Animator anim;
    private Transform currentPoint;

    public LayerMask whatIsPlayer;

    public float speed;
    public float attackRadius;

    public Encounter encounter;
    public LevelManager levelManager;

    private bool waiting = false;

    private bool isInAttackRange;
    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentPoint = pointB.transform;
        anim.SetBool("isRunning", true);
    }

    // Update is called once per frame
    private void Update()
    {
        if(!waiting)
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

            if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
            {
                flip();
                currentPoint = pointA.transform;
            }
            if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
            {
                flip();
                currentPoint = pointB.transform;
            }
            isInAttackRange = Physics2D.OverlapCircle(transform.position, attackRadius, whatIsPlayer);

        }
    }
    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("sceneBattle");
        }
    }

    private void FixedUpdate()
    {
        if (isInAttackRange)
        {
            EnterBattle();
        }
    }

    private void EnterBattle()
    {
        BattleSystem.currentEncounter = encounter;
        LevelManager.SetEnemy(this.gameObject);
        levelManager.UpdatePlayerPosition();
        LevelManager.currentEncounter = encounter;
        LevelManager.bgmSaveTime = BGMManager.instance.GetCurrentBGMTime();

        SceneManager.LoadScene("sceneBattle");
        
    }

    public IEnumerator WaitAfterFlee()
    {
        waiting = true;

        yield return new WaitForSeconds(3.5f);

        waiting = false;
    }
}
