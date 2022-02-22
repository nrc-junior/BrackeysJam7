using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mov : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D col;
    public float Speed;
    public bool isGrounded;
    Vector2 move;
    
    void Update()
    {
        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"),0);
        move = new Vector2(dir.x,dir.y);
        transform.localRotation = dir.x < 0 ? transform.localRotation = Quaternion.Euler(new Vector2(0,180)) : Quaternion.Euler(new Vector2(0,0));
        if(isGrounded == true){
            col.isTrigger = false;
        }
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position+move*Speed*Time.fixedDeltaTime);
    }
    void OnCollisionEnter2D(Collision2D collision){
        if(tag == "Ground"){
            isGrounded = true;
        }
    }
}
