using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }
    public State state = State.IDLE;
    public float traceDist = 10;
    public float attacDist = 2;

    public bool isDie = false;

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator anim;

    private readonly int hashTrace = Animator.StringToHash("isTrace");
    private readonly int hashAttack = Animator.StringToHash("isAttack");
    private readonly int hashHit = Animator.StringToHash("isHit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");

    public GameObject bloodEffect;
    private int hp = 100;

    private void OnEnable()
    {
        state = State.IDLE;
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction());
    }
    private void OnDisable()
    {

    }
    void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();

        //agent.destination = playerTr.position;
        anim = GetComponent<Animator>();

        bloodEffect = Resources.Load<GameObject>("Prefabs/BloodSprayEffect");

    }
    private void Update()
    {
        if (agent.remainingDistance >= 2f)
        {
            Vector3 direction = agent.desiredVelocity;
            Quaternion rot = Quaternion.LookRotation(direction);

            monsterTr.rotation = Quaternion.Slerp(monsterTr.rotation, rot, Time.deltaTime * 10f);
        }
    }
    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            yield return new WaitForSeconds(0.3f);
            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            if (state == State.DIE) yield break;

            if (distance <= attacDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;
                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;
                case State.ATTACK:
                    anim.SetBool(hashAttack, true);
                    break;
                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    anim.SetTrigger(hashDie);
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject);
            //anim.SetTrigger(hashHit);

            //Vector3 pos = collision.GetContact(0).point;
            //Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            //ShowBloodEffect(pos, rot);

            //hp -= 10;
            //if(hp <=0)
            //{
            //    state = State.DIE;
            //    GameManager.instance.DisplayScore(50);
            //}
        }
    }
    void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1.0f);
    }
    void InitMonster()
    {
        hp = 100;
        isDie = false;
        GetComponent<CapsuleCollider>().enabled = true;
        this.gameObject.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attacDist);
        }
    }

    void OnPlayerDie()
    {
        StopAllCoroutines();

        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));
        anim.SetTrigger(hashPlayerDie);

    }
    public void OnDamage(Vector3 pos, Vector3 normal)
    {
        anim.SetTrigger(hashHit);
        Quaternion rot = Quaternion.LookRotation(normal);

        ShowBloodEffect(pos, rot);

        hp -= 10;
        if (hp <= 0)
        {
            state = State.DIE;
        }
    }
}
