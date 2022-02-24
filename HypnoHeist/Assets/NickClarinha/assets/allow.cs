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
    }
   private void OnCollisionEnter2D(Collision2D other) {
       if(other.collider.tag == "Ground")
       {
           ob.GetComponent<PolygonCollider2D>().enabled = false;
            Debug.Log("bateu");
       }
   }
}
