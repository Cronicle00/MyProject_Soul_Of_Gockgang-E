using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailOnOff : MonoBehaviour
{
    public GameObject Trail;
    // Start is called before the first frame update
    void Start()
    {
        Trail.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTrail()
    {
        Trail.SetActive(true);    
    }
    public void OffTrail()
    {
        Trail.SetActive(false);
    }
}
