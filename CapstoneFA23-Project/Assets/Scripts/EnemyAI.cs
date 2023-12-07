using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    public float speed;
    public float checkRadius;
    public float attackRadius;

    public bool shouldRotate;
    public bool IsDefeated;

    public LayerMask whatIsPlayer;

    private Transform target;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    public Vector3 dir;

    private bool isInChaseRange;
    private bool isInAttackRange;

    public Encounter encounter;

    public LevelManager levelManager;

    private bool waiting = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
    }
    private void Update()
    {
        if(!waiting)
        {

            anim.SetBool("isRunning", isInChaseRange);

            isInChaseRange = Physics2D.OverlapCircle(transform.position, checkRadius, whatIsPlayer);
            isInAttackRange = Physics2D.OverlapCircle(transform.position, attackRadius, whatIsPlayer);

            dir = target.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            dir.Normalize();
            movement = dir;
            if(shouldRotate)
            {
                anim.SetFloat("X", dir.x);
                anim.SetFloat("Y", dir.y);
            }
        }
    }
    private void FixedUpdate()
    {
        if(isInChaseRange && !isInAttackRange)
        {
            MoveCharacter(movement);
        }
        if (isInAttackRange)
           EnterBattle();
    }

    private void MoveCharacter(Vector2 dir)
    {
        rb.MovePosition((Vector2)transform.position + (dir * speed * Time.deltaTime));
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
