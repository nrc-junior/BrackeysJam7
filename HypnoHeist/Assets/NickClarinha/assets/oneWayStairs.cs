using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneWayStairs : MonoBehaviour
{
    public bool isUp;
    private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow) && player.GetComponent<mov>().isGrounded){
            transform.parent.GetComponent<Collider2D>().enabled = false;
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player"){
            transform.parent.GetComponent<Collider2D>().enabled = isUp;
        }
    }
}
