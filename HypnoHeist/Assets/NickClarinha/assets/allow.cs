using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class allow : MonoBehaviour
{
    public GameObject ob;
    public BoxCollider2D col;
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey("up"))
        {
            ob.GetComponent<PolygonCollider2D>().enabled = true;
        }else if(Input.GetKey("down")){ob.GetComponent<PolygonCollider2D>().enabled = false;}
        if(col.GetComponent<Collider>() == true){ob.GetComponent<PolygonCollider2D>().enabled = false;}
    }
}
