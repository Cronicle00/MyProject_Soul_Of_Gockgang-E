using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimEvent : MonoBehaviour
{
    public MonsterController monsterController;
    public void Atk()
    {
        monsterController.PlayerDamageByEnemy();
    }
    public void MoveSpeedChanger()
    {
        monsterController.speed = 5;
    }
}
