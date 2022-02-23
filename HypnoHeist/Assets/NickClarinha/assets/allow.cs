using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class allow : MonoBehaviour
{
    public GameObject ob;
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("up"))
        {
            ob.GetComponent<PolygonCollider2D>().enabled = true;
        }else{ob.GetComponent<PolygonCollider2D>().enabled = false;}
    }
}
