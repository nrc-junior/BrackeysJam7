using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugallow : MonoBehaviour {
    public GameObject ob;
    public BoxCollider2D col;
    
    void Update() {
        if(Input.GetKey("up")) {
            ob.GetComponent<PolygonCollider2D>().enabled = true;
        } else if (Input.GetKey("down")) {
            ob.GetComponent<PolygonCollider2D>().enabled = false;
        }

        if (col.GetComponent<Collider>() == true) {
            ob.GetComponent<PolygonCollider2D>().enabled = false;
        }
    }
}
