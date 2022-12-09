using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Trail : MonoBehaviour
{
    [Header("Weapon FX")]
    public ParticleSystem normalWeaponTrail;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayWeaponFx()
    {
        normalWeaponTrail.Stop();
        if(normalWeaponTrail.isStopped)
        {
            normalWeaponTrail.Play();
        }
    }
}
