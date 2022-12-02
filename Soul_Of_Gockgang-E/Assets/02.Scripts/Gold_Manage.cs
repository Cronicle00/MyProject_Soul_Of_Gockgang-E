using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold_Manage : MonoBehaviour
{
    public int gold = 0;
    private PlayerController playerController;

    public enum MONSTERGRADE
    {
        NORMAL,
        HARD,
        EPIC,
        BOSS
    }

    public MONSTERGRADE monsterGrade;

    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        CoinPrice();
    }
    public void CoinPrice()
    {
        switch(monsterGrade)
        {
            case MONSTERGRADE.NORMAL:
                gold = Random.Range(5, 10);
                break;
            case MONSTERGRADE.HARD:
                gold = Random.Range(10, 25);
                break;
            case MONSTERGRADE.EPIC:
                gold = Random.Range(25, 50);
                break;
            case MONSTERGRADE.BOSS:
                gold = Random.Range(100, 500);
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Added Gold");
            playerController.gold += gold;
            Destroy(gameObject);
        }
    } 
}
