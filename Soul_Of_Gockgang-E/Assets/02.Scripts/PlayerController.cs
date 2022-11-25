using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
        //float h = Input.GetAxisRaw("Horizontal");
        //float v = Input.GetAxisRaw("Vertical");

        //Vector3 moveDirection = new Vector3(h, 0, v);

        //moveDirection *= moveSpeed;

        Vector3 forward = tPSCam.transform.TransformDirection(Vector3.forward);
        Vector3 right = tPSCam.transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

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

        //characterController.Move(moveDirection * Time.deltaTime);
        characterController.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);

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
                isATK = false;
                moveSpeed = 5f;
                jumpSpeed = 10f;
                weapon_System.isAttack = false;
                InputChecker();
                break;

            #region KEYUP
            case PLAYERSTATE.RUN:
                isATK = false;
                weapon_System.isAttack = false;
                if (Input.GetKeyUp(KeyCode.W))
                {
                    IdleState();
                }
                InputChecker();
                break;
            case PLAYERSTATE.RUNBACK:
                isATK = false;
                if (Input.GetKeyUp(KeyCode.S))
                {
                    IdleState();
                }
                InputChecker();
                break;
            case PLAYERSTATE.MOVER:
                isATK = false;
                if (Input.GetKeyUp(KeyCode.D))
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
                InputChecker();
                break;
            case PLAYERSTATE.MOVEL:
                isATK = false;
                if (Input.GetKeyUp(KeyCode.A))
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
                InputChecker();
                break;
            #endregion

            case PLAYERSTATE.JUMP:
                IdleState();
                isIdle = false;
                isATK = false;
                break;
            case PLAYERSTATE.ATTACK_IDLE:
                isATK_Idle = true;
                isIdle = false;
                moveSpeed = 5f;
                jumpSpeed = 10f;
                weapon_System.isAttack = false;
                isGuard = false;
                isDamaged = false;
                isATK = false;
                if (Input.GetKey(KeyCode.Mouse0))
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
                isATK = true;
                moveSpeed = 0;
                jumpSpeed = 0;
                playerState = PLAYERSTATE.ATTACK_IDLE;
                break;

            case PLAYERSTATE.TURNSLASH:
                isIdle = false;
                isGuard = false;
                isATK = true;
                break;

            case PLAYERSTATE.BLOCK:
                isIdle = false;
                isGuard = true;
                moveSpeed = 0;
                jumpSpeed = 0;
                weapon_System.isAttack = false;
                isATK = false;
                if (Input.GetKeyUp(KeyCode.Mouse1)||stamina<=0)
                {
                    ableGuard = false;
                    playerState = PLAYERSTATE.ATTACK_IDLE;
                }
                break;
            case PLAYERSTATE.RUNWITHSWORD:
                weapon_System.isAttack = false;
                isATK = false;
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
                isATK = false;
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
                isATK = false;
                moveSpeed = 0;
                jumpSpeed = 0;
                break;

            default:
                break;
        }
        
        playerAnim.SetInteger("PLAYERSTATE", (int)playerState);
        playerAnim.SetBool("ableGuard", ableGuard);
    }

    public float smoothness = 10f;
    public void LateUpdate()
    {
        if(!isGuard||!isATK)
        {
            Vector3 playerRotate = Vector3.Scale(tPSCam.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }
    public void InputChecker()
    {
        if(!isDamaged || !isGuard || !isATK)
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
        isATK = true;
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
