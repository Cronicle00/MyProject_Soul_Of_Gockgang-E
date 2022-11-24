using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform cameraTransform;
    public GameObject tPSCam;

    public float moveSpeed = 5.0f;
    public float rotateSpeed = 10.0f;
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
    private bool ableGuard = true;
    private Animator playerAnim;
    public SheildController sheildController;
    public GameObject sheildHitVFX;
    public GameObject sheildPos;

    public float hp = 10;
    private float maxHp =0;
    public float stamina = 10;
    private float maxStamina = 0;
    public float mental = 10;
    public int gold = 0;

    private float stamina_timer;
    private bool timerActive;
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
        maxHp = hp;
        maxStamina = stamina;
        timerActive = false;
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

        if (!(h == 0 && v == 0) && !isGuard)
        {
            // 이동과 회전을 함께 처리
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            // 회전하는 부분. Point 1.
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDirection), Time.deltaTime * rotateSpeed);
        }

        moveDirection *= moveSpeed;

        if (characterController.isGrounded)
        {
            yVelocity = 0;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Debug.Log("jump");
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

        if(stamina >= 0.5f)
        {
            ableGuard = true;
        }

        RefillStamina();

        switch (playerState)
        {
            case PLAYERSTATE.IDLE:
                isGuard = false;
                isATK_Idle = false;
                moveSpeed = 5f;
                jumpSpeed = 10f;
                weapon_System.isAttack = false;
                //player.transform.rotation = Quaternion.identity;
                InputChecker();
                break;

            #region KEYUP
            case PLAYERSTATE.RUN:
                weapon_System.isAttack = false;
                if (Input.GetKeyUp(KeyCode.W)|| Input.GetKeyUp(KeyCode.A)|| Input.GetKeyUp(KeyCode.S)|| Input.GetKeyUp(KeyCode.D))
                {
                    IdleState();
                }
                break;
            case PLAYERSTATE.RUNBACK:
                //if (Input.GetKeyUp(KeyCode.S))
                //{
                //    IdleState();
                //}
                break;
            case PLAYERSTATE.MOVER:
                //if(Input.GetKeyUp(KeyCode.D))
                //{
                //    if (Input.GetKey(KeyCode.A))
                //    {
                //        playerState = PLAYERSTATE.MOVEL;
                //    }
                //    else
                //    {
                //        IdleState();
                //    }
                //}
                break;
            case PLAYERSTATE.MOVEL:
                //if(Input.GetKeyUp(KeyCode.A))
                //{
                //    if (Input.GetKey(KeyCode.D))
                //    {
                //        playerState = PLAYERSTATE.MOVER;
                //    }
                //    else
                //    {
                //        IdleState();
                //    }
                //}
                break;
            #endregion

            case PLAYERSTATE.JUMP:
                IdleState();
                isIdle = false;
                break;
            case PLAYERSTATE.ATTACK_IDLE:
                isATK_Idle = true;
                isIdle = false;
                moveSpeed = 5f;
                jumpSpeed = 10f;
                weapon_System.isAttack = false;
                isGuard = false;
                isDamaged = false;
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
                moveSpeed = 0;
                jumpSpeed = 0;
                playerState = PLAYERSTATE.ATTACK_IDLE;
                break;

            case PLAYERSTATE.TURNSLASH:
                isIdle = false;
                isGuard = false;
                break;

            case PLAYERSTATE.BLOCK:
                isIdle = false;
                isGuard = true;
                moveSpeed = 0;
                jumpSpeed = 0;
                weapon_System.isAttack = false;
                if (Input.GetKeyUp(KeyCode.Mouse1)||stamina<=0)
                {
                    ableGuard = false;
                    playerState = PLAYERSTATE.ATTACK_IDLE;
                }
                break;
            case PLAYERSTATE.RUNWITHSWORD:
                weapon_System.isAttack = false;
                if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
                {
                    IdleState();
                }
                InputChecker();
                break;
            case PLAYERSTATE.DAMAGED:
                isDamaged = true;
                isATK_Idle = true;
                weapon_System.isAttack = false;
                moveSpeed = 0;
                jumpSpeed = 0;
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
                weapon_System.isAttack = false;
                moveSpeed = 0;
                jumpSpeed = 0;
                break;

            default:
                break;
        }
        
        playerAnim.SetInteger("PLAYERSTATE", (int)playerState);
        playerAnim.SetBool("ableGuard", ableGuard);
    }

    public void InputChecker()
    {
        if(!isDamaged || !isGuard)
        {
            if (Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                RunState();
            }
            //if (Input.GetKey(KeyCode.S))
            //{
            //    playerState = PLAYERSTATE.RUNBACK;

            //}
            //if (Input.GetKey(KeyCode.D))
            //{
            //    playerState = PLAYERSTATE.MOVER;

            //}
            //if (Input.GetKey(KeyCode.A))
            //{
            //    playerState = PLAYERSTATE.MOVEL;

            //}
            //if(Input.GetKey(KeyCode.Space))
            //{
            //    playerState = PLAYERSTATE.JUMP;
            //}
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerState = PLAYERSTATE.ATTACK_IDLE;
            }
            if(stamina > 0)
            {
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    playerState = PLAYERSTATE.BLOCK;
                }
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
        moveSpeed = 0;
        jumpSpeed = 0;
        weapon_System.isAttack = true;
        playerAnim.SetTrigger("SLASH_COMBO");
    }
    public void DamageByEnemy()
    {
        if(!isGuard)
        {
            hp--;
            playerState = PLAYERSTATE.DAMAGED;
        }
        else if(stamina > 0)
        {
            stamina -= 0.5f;
            GameObject hitvfx = Instantiate<GameObject>(sheildHitVFX, sheildPos.transform.position, sheildPos.transform.rotation);
            Destroy(hitvfx, 1.0f);
            //playerState = PLAYERSTATE.BLOCK;
        }
        else
        {
            isGuard = false;
            ableGuard = false;
            IdleState();
        }
    }
    private void RefillStamina()
    {
        if(stamina < maxStamina)
        {
            StartCoroutine(CoStaminaTimer());
        }
    }

    private IEnumerator CoStaminaTimer()
    {
        stamina_timer = 2f;
        timerActive = true;
        while(stamina_timer > 0&&timerActive)
        {
            stamina_timer -= Time.deltaTime;
            yield return null;

            if (stamina_timer<=0)
            {
                while(stamina == maxStamina)
                {
                    stamina += 0.1f*Time.deltaTime;
                }

                timerActive = false;
            }
        }
        
    }
}
