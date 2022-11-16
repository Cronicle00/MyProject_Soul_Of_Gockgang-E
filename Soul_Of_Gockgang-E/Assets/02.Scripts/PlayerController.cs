using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    Vector3 moveDirection;

    private CharacterController characterController;
    public GameObject player;
    public bool isIdle = false;
    public bool isJump = false;
    public bool isDead = false;
    public bool isDamaged = false;
    public bool isATK = false;
    public bool isATK_Idle = false;
    public bool isGuard = false;
    private Animator playerAnim;

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
        DEAD
    }
    public PLAYERSTATE playerState;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerAnim = gameObject.GetComponentInChildren<Animator>();
        playerState = PLAYERSTATE.IDLE;
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(x, 0, z);

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        switch(playerState)
        {
            case PLAYERSTATE.IDLE:
                isGuard = false;
                isATK_Idle = false;
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
                isIdle = false;
                break;
            case PLAYERSTATE.ATTACK_IDLE:
                isATK_Idle = true;
                isIdle = false;
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
                if(Input.GetKeyUp(KeyCode.Mouse1))
                {
                    playerState = PLAYERSTATE.ATTACK_IDLE;
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
        if(isGuard == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerState = PLAYERSTATE.JUMP;

            }
            if (Input.GetKey(KeyCode.W))
            {
                playerState = PLAYERSTATE.RUN;
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
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                playerState = PLAYERSTATE.ATTACK_IDLE;
            }
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            playerState = PLAYERSTATE.BLOCK;
            isGuard = true;
        }
    }
    public void IdleState()
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
    public void Attacking()
    {
        playerAnim.SetTrigger("SLASH_COMBO");
    }
}
