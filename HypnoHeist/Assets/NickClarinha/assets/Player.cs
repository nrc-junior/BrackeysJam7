using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public BoxCollider2D col;
    public Transform GroundChechCollider;
    public LayerMask GroundLayer;
    public bool inFly = true;
    public float Speed;
    private float SlopeDownOld;
    public bool isGrounded;
    private CircleCollider2D cc;
    private Vector2 move;
    private Vector2 ColliderSize;
    private float DownAngle;
    private float SloepSideAngle;
    private Vector2 NormalPerp;
    [SerializeField] private float CheckDistance;
    public bool hidden = false;
    private bool isSlope;
    [SerializeField]  private LayerMask whatIsGround;
    
    
    private void Start()
    {
        cc = GetComponent<CircleCollider2D>();

        ColliderSize = cc.offset;
    }
    private void SlopeCheck()
    {
        Vector2 CheckPos = transform.position - new Vector3(0.0f, ColliderSize.y/2);
        CheckVertical(CheckPos);
        CheckHorizontal(CheckPos);
    }
    private void CheckHorizontal(Vector2 CheckPos)
    {
        RaycastHit2D hitFront = Physics2D.Raycast(CheckPos, transform.right, CheckDistance, whatIsGround);
        RaycastHit2D hitBack = Physics2D.Raycast(CheckPos, -transform.right, CheckDistance, whatIsGround);

        if(hitFront)
        {
            isSlope = true;
            SloepSideAngle = Vector2.Angle(hitFront.normal, Vector2.up);
        }else if (hitBack)
        {
            isSlope = true;
            SloepSideAngle = Vector2.Angle(hitBack.normal, Vector2.up);
        }else
        {
            SloepSideAngle = 0.0f;
            isSlope = false;
        }
    }
    private void CheckVertical(Vector2 CheckPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(CheckPos, Vector2.down,CheckDistance,whatIsGround);
        if (hit)
        {
            NormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            DownAngle = Vector2.Angle(hit.normal,Vector2.up);
            if(DownAngle != SlopeDownOld){isSlope = true;}
            SlopeDownOld = DownAngle;
            Debug.DrawRay(hit.point, NormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
            isGrounded = true;
        }
    }
    void Update()
    {
        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"),0);
        move = new Vector2(dir.x,dir.y);
        transform.localRotation = dir.x < 0 ? transform.localRotation = Quaternion.Euler(new Vector2(0,180)) : Quaternion.Euler(new Vector2(0,0));
        if(isGrounded && !isSlope)
        {
            move.Set(Speed*dir.x,0.0f);
            rb.velocity = move;
        }else if(isGrounded && isSlope)
        {
            move.Set(Speed*NormalPerp.x*-dir.x, Speed*NormalPerp.y*-dir.x);
            rb.velocity = move;
        }else if(!isGrounded)
        {
            move.Set(Speed*dir.x, rb.velocity.y);
            rb.velocity = move;
        }
    }
    void FixedUpdate()
    {
        rb.MovePosition(rb.position+move*Speed*Time.fixedDeltaTime);
        SlopeCheck();
        GroundChech();
        if (inFly == true){rb.AddForce(Vector2.down*500);}
    }
   
   void GroundChech()
   {
       inFly = true;
       Collider2D[] collider = Physics2D.OverlapCircleAll(GroundChechCollider.position,0.2f,GroundLayer);
       if(collider.Length > 0){inFly = false;}
   }
    private void OnCollisionEnter2D(Collision2D other) {
       if(other.collider.tag == "Ground")
       {
            col.isTrigger = true;
       }
       if(other.collider.tag == "Stairs"){col.isTrigger = false;}
   }
}
