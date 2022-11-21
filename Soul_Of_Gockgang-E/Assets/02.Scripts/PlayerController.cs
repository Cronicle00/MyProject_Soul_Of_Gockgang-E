using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform cameraTransform;

    public float moveSpeed = 5.0f;
    private Vector3 moveDirection = Vector3.zero;
    public float jumpSpeed = 10f;
    public float gravity = -20f;
    public float yVelocity = 0;
    public float stateTime;

    private CharacterController characterController;
    public Weapon_System weapon_System;
    public GameObject player;
    private bool isIdle = false;
    private bool isJump = false;
    private bool isDead = false;
    private bool isDamaged = false;
    private bool isATK = false;
    private bool isATK_Idle = false;
    private bool isGuard = false;
    private Animator playerAnim;

    public int hp = 10;
    public enum PLAYERSTATE
    {
        IDLE =0,
        RUN,
        RUNBACK,
        MOVER,
        MOVEL,
        JUMP,
        ATTACK_IDLE,
        ATTACK,
        TURNSLASH,
        BLOCK,
        RUNWITHSWORD = 10,
        DAMAGED,
        DEAD
    }
    public PLAYERSTATE playerState;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = transform.GetChild(0);
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        playerState = PLAYERSTATE.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead)
        {
            return;
        }
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = new Vector3(h, 0, v);

        
        moveDirection *= moveSpeed;

        if (characterController.isGrounded)
        {
            yVelocity = 0;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("jump");
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += (gravity * Time.deltaTime);
        moveDirection.y = yVelocity;

        characterController.Move(moveDirection * Time.deltaTime);

        if (transform.position.y < -2f)
        {
            transform.position = new Vector3(0, 20, 0);
        }

        switch (playerState)
        {
            case PLAYERSTATE.IDLE:
                isGuard = false;
                isATK_Idle = false;
                weapon_System.isAttack = false;
                player.transform.rotation = Quaternion.identity;
                InputChecker();
                break;

            #region KEYUP
            case PLAYERSTATE.RUN:
                if(Input.GetKeyUp(KeyCode.W))
                {
                    IdleState();
                }
                break;
            case PLAYERSTATE.RUNBACK:
                if (Input.GetKeyUp(KeyCode.S))
                {
                    IdleState();
                }
                break;
            case PLAYERSTATE.MOVER:
                if(Input.GetKeyUp(KeyCode.D))
                {
                    if (Input.GetKey(KeyCode.A))
                    {
                        playerState = PLAYERSTATE.MOVEL;
                    }
                    else
                    {
                        IdleState();
                    }
                }
                break;
            case PLAYERSTATE.MOVEL:
                if(Input.GetKeyUp(KeyCode.A))
                {
                    if (Input.GetKey(KeyCode.D))
                    {
                        playerState = PLAYERSTATE.MOVER;
                    }
                    else
                    {
                        IdleState();
                    }
                }
                break;
            #endregion

            case PLAYERSTATE.JUMP:
                IdleState();
                isIdle = false;
                break;
            case PLAYERSTATE.ATTACK_IDLE:
                isATK_Idle = true;
                isIdle = false;
                weapon_System.isAttack = false;
                isGuard = false;
                if(Input.GetKey(KeyCode.Mouse0))
                {
                    Attacking();
                    playerState = PLAYERSTATE.ATTACK;
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    playerState = PLAYERSTATE.BLOCK;
                }
                InputChecker();
                break;

            case PLAYERSTATE.ATTACK:
                isIdle = false;
                playerState = PLAYERSTATE.ATTACK_IDLE;
                break;

            case PLAYERSTATE.TURNSLASH:
                isIdle = false;
                break;

            case PLAYERSTATE.BLOCK:
                isIdle = false;
                weapon_System.isAttack = false;
                if (Input.GetKeyUp(KeyCode.Mouse1))
                {
                    playerState = PLAYERSTATE.ATTACK_IDLE;
                }
                break;
            case PLAYERSTATE.RUNWITHSWORD:
                if (Input.GetKeyUp(KeyCode.W))
                {
                    IdleState();
                }
                InputChecker();
                break;
            case PLAYERSTATE.DAMAGED:
                isDamaged = true;
                stateTime += Time.deltaTime;
                if (stateTime > 1f)
                {
                    stateTime = 0;
                    IdleState();
                }
                if (hp <= 0)
                {
                    playerState = PLAYERSTATE.DEAD;
                }
                break;
            case PLAYERSTATE.DEAD:
                isIdle = false;
                break;

            default:
                break;
        }
        
        playerAnim.SetInteger("PLAYERSTATE", (int)playerState);
    }

    public void InputChecker()
    {
        if(!isDamaged)
        {
            if (Input.GetKey(KeyCode.W))
            {
                RunState();
            }
            if (Input.GetKey(KeyCode.S))
            {
                playerState = PLAYERSTATE.RUNBACK;

            }
            if (Input.GetKey(KeyCode.D))
            {
                playerState = PLAYERSTATE.MOVER;

            }
            if (Input.GetKey(KeyCode.A))
            {
                playerState = PLAYERSTATE.MOVEL;

            }
            //if(Input.GetKey(KeyCode.Space))
            //{
            //    playerState = PLAYERSTATE.JUMP;
            //}
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerState = PLAYERSTATE.ATTACK_IDLE;
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                playerState = PLAYERSTATE.BLOCK;
                isGuard = true;
            }
        }
    }
    private void IdleState()
    {
        if (isATK_Idle)
        {
            playerState = PLAYERSTATE.ATTACK_IDLE;
        }
        else
        {
            playerState = PLAYERSTATE.IDLE;
        }
    }
    private void RunState()
    {
        if (isATK_Idle)
        {
            playerState = PLAYERSTATE.RUNWITHSWORD;
        }
        else
        {
            playerState = PLAYERSTATE.RUN;
        }
    }
    public void Attacking()
    {
        weapon_System.isAttack = true;
        playerAnim.SetTrigger("SLASH_COMBO");
    }
    public void DamageByEnemy()
    {
        hp--;
        playerState = PLAYERSTATE.DAMAGED;
    }
}
