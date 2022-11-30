using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetStateChanger : MonoBehaviour
{
    public List<MonsterController> stageMonsters;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            MonsterStateChangerIN();
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        MonsterStateChangerOUT();
    //    }
    //}
    public void MonsterStateChangerIN()
    {
        for (int i = 0; i < stageMonsters.Count; i++)
        {
            stageMonsters[i].activeTracking = true;
        }
    }
    public void MonsterStateChangerOUT()
    {
        for (int i = 0; i < stageMonsters.Count; i++)
        {
            stageMonsters[i].activeTracking = false;
        }
    }
}
