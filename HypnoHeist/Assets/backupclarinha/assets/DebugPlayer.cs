using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayer : MonoBehaviour {
    private Rigidbody2D rb;
    private BoxCollider2D col;
    public Transform groundCheckCol;
    public LayerMask GroundLayer;

    public bool m_FacingRight = true;
    public bool inFly = true;
    public float Speed;
    private float SlopeDownOld;
    public bool isGrounded;
    private CircleCollider2D cc;
    private Vector2 move;
    private Vector2 ColliderSize;
    private float DownAngle;
    private float SlopeSideAngle;
    private Vector2 NormalPerp;
    
    [SerializeField] private float CheckDistance;
    private Collider2D oldStair; // ultimo collider com tag "Stairs" que o player pisou 
    
    public bool hidden = false;
    private bool isSlope;
    [SerializeField]  private LayerMask whatIsGround;
    
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        
        cc = GetComponent<CircleCollider2D>();

        ColliderSize = cc.offset;
    }
    
    void Update() {
        Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"),0);
        move = new Vector2(dir.x,dir.y);

        if (move.x > 0 && !m_FacingRight) { // NRC: flip flop por size.
            Flip();
        } else if (move.x < 0 && m_FacingRight) {
            Flip();
        }
        
        if (isGrounded && !isSlope) {
            move.Set(Speed*dir.x,0.0f);
            rb.velocity = move;
            
        } else if(isGrounded && isSlope) {
            move.Set(Speed*NormalPerp.x*-dir.x, Speed*NormalPerp.y*-dir.x);
            rb.velocity = move;
            
        } else if(!isGrounded) {
            move.Set(Speed*dir.x, rb.velocity.y);
            rb.velocity = move;
        }
    }
    
    void FixedUpdate() {
        rb.MovePosition(rb.position+move*Speed*Time.fixedDeltaTime);
        SlopeCheck();
        GroundCheck();
        
        if (inFly) {
            rb.AddForce(Vector2.down*500);
        }
    }
    
    
    private void Flip() {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    
    private void SlopeCheck()
    {
        Vector2 CheckPos = transform.position - new Vector3(0.0f, ColliderSize.y/2);
        CheckVertical(CheckPos);
        CheckHorizontal(CheckPos);
    }
    
    private void CheckHorizontal(Vector2 CheckPos) {
        RaycastHit2D hitFront = Physics2D.Raycast(CheckPos, transform.right, CheckDistance, whatIsGround);
        RaycastHit2D hitBack = Physics2D.Raycast(CheckPos, -transform.right, CheckDistance, whatIsGround);
        if(hitFront) {
            isSlope = true;
            SlopeSideAngle = Vector2.Angle(hitFront.normal, Vector2.up);
            
        } else if (hitBack) {
            isSlope = true;
            SlopeSideAngle = Vector2.Angle(hitBack.normal, Vector2.up);
            
        } else {
            SlopeSideAngle = 0.0f;
            isSlope = false;
        }
    }
    
    private void CheckVertical(Vector2 CheckPos) {
        RaycastHit2D hit = Physics2D.Raycast(CheckPos, Vector2.down,CheckDistance,whatIsGround);
        if (hit) {
            NormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            DownAngle = Vector2.Angle(hit.normal,Vector2.up);

            if (DownAngle != SlopeDownOld) {
                isSlope = true;
            }
            
            SlopeDownOld = DownAngle;
            isGrounded = true;

            if (hit.collider.CompareTag("Stairs")) { // NRC: compara tag do collider que ta em baixo do player 
                oldStair = hit.collider;
            } else if(oldStair != null) {
                oldStair.enabled = false;
            }
            
            Debug.DrawRay(hit.point, NormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }
    

   
   void GroundCheck() {
       inFly = true;
       Collider2D[] collider = Physics2D.OverlapCircleAll(groundCheckCol.position,0.2f,GroundLayer);

       if(collider.Length > 0) inFly = false;
   }

}
