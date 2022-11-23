using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheildController : MonoBehaviour
{
    public GameObject sheildHitVFx;
    
    public void SheildHit()
    {
        GameObject hitvfx = Instantiate<GameObject>(sheildHitVFx, transform.position, transform.rotation);
        Destroy(hitvfx, 1.0f);
    }
}
