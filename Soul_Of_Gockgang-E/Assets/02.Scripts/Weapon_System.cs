using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_System : MonoBehaviour
{
    public bool isAttack = false;
    public GameObject sandBagHitVfx;
    public GameObject bloodVfx;
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
        isAttack = false;
        LoadWeaponData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(isAttack)
        {
            if (other.CompareTag("SandBag"))
            {
                SandBagVFX();
            }
            if(other.CompareTag("Enemy"))
            {
                BloodVFX();
            }
        }
    }
    void SandBagVFX()
    {
        GameObject hitvfx = Instantiate<GameObject>(sandBagHitVfx, transform.position, transform.rotation);
        Destroy(hitvfx, 1.0f);
    }
    void BloodVFX()
    {
        GameObject blood = Instantiate<GameObject>(bloodVfx, transform.position, transform.rotation);
        Destroy(blood, 1.0f);
    }
}
