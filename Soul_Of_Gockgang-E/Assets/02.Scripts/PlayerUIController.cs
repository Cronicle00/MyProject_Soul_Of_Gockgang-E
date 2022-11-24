using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public PlayerController playerController;
    public Slider playerHP;
    public Slider stamina;
    void Start()
    {
    }


    void Update()
    {
        playerHP.value = (float)playerController.hp;
        stamina.value = (float)playerController.stamina;
    }
}
