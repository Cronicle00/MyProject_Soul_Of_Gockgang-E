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
        ATTACK,
        TURNSLASH,
        GUARD,
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
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerState = PLAYERSTATE.JUMP;
                    
                }
                if(Input.GetKeyDown(KeyCode.W))
                {
                    playerState = PLAYERSTATE.RUN;
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    playerState = PLAYERSTATE.RUNBACK;
                }
                if(Input.GetKeyDown(KeyCode.D))
                {
                    playerState = PLAYERSTATE.MOVER;
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    playerState = PLAYERSTATE.MOVEL;
                }
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
                    playerState = PLAYERSTATE.IDLE;
                }
                break;
            case PLAYERSTATE.MOVEL:
                if(Input.GetKeyUp(KeyCode.A))
                {
                    playerState = PLAYERSTATE.IDLE;
                }
                break;
            #endregion

            case PLAYERSTATE.JUMP:
                isIdle = false;
                break;

            case PLAYERSTATE.ATTACK:
                isIdle = false;
                break;

            case PLAYERSTATE.TURNSLASH:
                isIdle = false;
                break;

            case PLAYERSTATE.GUARD:
                isIdle = false;
                break;

            case PLAYERSTATE.DEAD:
                isIdle = false;
                break;

            default:
                break;
        }
        playerAnim.SetInteger("PLAYERSTATE", (int)playerState);
    }
}
