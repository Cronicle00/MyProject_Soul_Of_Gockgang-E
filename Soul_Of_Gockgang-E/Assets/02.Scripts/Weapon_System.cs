using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_System : MonoBehaviour
{
    
    public string dataPath;
    public List<Dictionary<string, object>> data;
    public enum WEAPONTYPE
    {
        SWORD,
        RAPIER,
        HAMMAER,
        GOCKGANG_E
    }
    public void LoadWeaponData()
    {
        dataPath = "WEAPON";
        data = CSVReader.Read(dataPath);
        Debug.Log($"불러온 데이터 갯수 ::: {data.Count}");
    }
    void Start()
    {
        LoadWeaponData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
