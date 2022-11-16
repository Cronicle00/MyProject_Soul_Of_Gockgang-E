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
                player.transform.rotation = Quaternion.identity;
                InputChecker();
                break;

            #region KEYUP
            case PLAYERSTATE.RUN:
                if(Input.GetKeyUp(KeyCode.W))
                {
                    playerState = PLAYERSTATE.IDLE;
                }
                break;
            case PLAYERSTATE.RUNBACK:
                if (Input.GetKeyUp(KeyCode.S))
                {
                    playerState = PLAYERSTATE.IDLE;
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
                        playerState = PLAYERSTATE.IDLE;
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
                        playerState = PLAYERSTATE.IDLE;
                    }
                }
                break;
            #endregion

            case PLAYERSTATE.JUMP:
                isIdle = false;
                break;
            case PLAYERSTATE.ATTACK_IDLE:
                isIdle = false;
                if(Input.GetKey(KeyCode.Mouse0))
                {
                    playerState = PLAYERSTATE.ATTACK;
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    playerState = PLAYERSTATE.BLOCK;
                    player.transform.rotation = Quaternion.Euler(0, 58f, 0);
                }
                break;

            case PLAYERSTATE.ATTACK:
                isIdle = false;
                    playerState = PLAYERSTATE.ATTACK_IDLE;
                    player.transform.rotation = Quaternion.Euler(0, 41f, 0);
                break;

            case PLAYERSTATE.TURNSLASH:
                isIdle = false;
                break;

            case PLAYERSTATE.BLOCK:
                isIdle = false;
                if(Input.GetKeyUp(KeyCode.Mouse1))
                {
                    playerState = PLAYERSTATE.ATTACK_IDLE;
                    player.transform.rotation = Quaternion.Euler(0, 41f, 0);
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
            player.transform.rotation = Quaternion.Euler(0f, 28.697f, 0f);
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerState = PLAYERSTATE.MOVER;
            player.transform.rotation = Quaternion.Euler(0, 60f, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerState = PLAYERSTATE.MOVEL;
            player.transform.rotation = Quaternion.Euler(0, 60f, 0);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            playerState = PLAYERSTATE.ATTACK_IDLE;
            player.transform.rotation = Quaternion.Euler(0, 41f, 0);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            playerState = PLAYERSTATE.BLOCK;
            player.transform.rotation = Quaternion.Euler(0, 58f, 0);
        }
    }
}
