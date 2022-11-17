using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour
{
    public enum ENEMYSTATE
    {
        IDLE = 0,
        MOVE,
        ATTACK,
        DAMAGE,
        DEAD
    }

    public ENEMYSTATE enemyState;
    public float stateTime;
    public float idleStateTime;
    public Animator enemyAnim;
    public Transform target;

    public float speed = 0.5f;
    public float rotationSpeed = 10f;
    public float attackRange = 2.5f;
    public float attackStateMaxTime;

    public CharacterController enemyCharacterController;

    public int hp = 5;
    public PlayerController playerState;
    // Start is called before the first frame update
    void Start()
    {
        enemyState = ENEMYSTATE.IDLE;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        enemyCharacterController = GetComponent<CharacterController>();
        enemyAnim = GetComponentInChildren<Animator>();
        playerState = target.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyState)
        {
            case ENEMYSTATE.IDLE:
                stateTime += Time.deltaTime;
                if (stateTime > idleStateTime)
                {
                    stateTime = 0;
                    enemyState = ENEMYSTATE.MOVE;
                }
                break;
            case ENEMYSTATE.MOVE:
                float distance = Vector3.Distance(target.position, transform.position);
                if (distance < attackRange)
                {
                    enemyState = ENEMYSTATE.ATTACK;
                    stateTime = 0;
                }
                else
                {
                    Vector3 dir = target.position - transform.position;
                    dir.y = 0;
                    dir.Normalize();
                    enemyCharacterController.SimpleMove(dir * speed);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
                }
                break;
            case ENEMYSTATE.ATTACK:
                stateTime += Time.deltaTime;
                if (stateTime > attackStateMaxTime)
                {
                    Debug.Log("Attack");
                    //playerState.DamageByEnemy();
                    stateTime = 0;
                }
                float dist = Vector3.Distance(target.position, transform.position);
                if (dist > attackRange)
                {
                    enemyState = ENEMYSTATE.MOVE;
                    stateTime = 0;
                }
                break;
            case ENEMYSTATE.DAMAGE:
                stateTime += Time.deltaTime;
                if (stateTime > 1f)
                {
                    stateTime = 0;
                    enemyState = ENEMYSTATE.MOVE;
                }
                if (hp <= 0)
                {
                    enemyState = ENEMYSTATE.DEAD;
                }
                break;
            case ENEMYSTATE.DEAD:
                enemyCharacterController.enabled = false;
                Destroy(gameObject, 3f);
                break;
            default:
                break;
        }

        enemyAnim.SetInteger("EnemyState", (int)enemyState);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Sword")
        {
            --hp;
            enemyState = ENEMYSTATE.DAMAGE;
        }
    }
}
