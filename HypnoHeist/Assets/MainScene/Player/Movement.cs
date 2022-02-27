using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Movement : MonoBehaviour {
    private Rigidbody2D rb;    
    private BoxCollider2D col;

    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] Transform groundCheckCol;
    [SerializeField] private float checkDistance;
    bool isGrounded;
    
    public bool m_FacingRight = true;
    public bool inFly = true;
    public float Speed;
    private float SlopeDownOld;

    private CircleCollider2D cc;
    
    
    private bool isSlope;
    private Vector2 colliderSize;
    private float downAngle;
    private Vector2 normalPerp;
    private float slopeSideAngle;
    
    private Vector2 move;
    public bool climbing;
    private bool isPlayer;
    private PlayerBehaviours pb;
    private Collider2D oldStair; // ultimo collider com tag "Stairs" que o player/bot pisou

    public delegate void Turns();
    public event Turns ParentTurn;
    
    public delegate void CleanBehaviours();
    public static event CleanBehaviours PlayerBehaviours;

    bool stun = false;

    // hypnosis 
    bool iAmTarget = false;

    private Animator animator;
    
    private void Awake() {
        animator = GetComponent<Animator>();
        
        isPlayer = CompareTag("Player");
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        cc = GetComponent<CircleCollider2D>();

        var polygonCols = FindObjectsOfType<PolygonCollider2D>();
        foreach (var _col in polygonCols) {
            if (_col.CompareTag("Stairs")) {
                IgnoreCollision(_col);
            }
        }
        
        if (isPlayer) {
            pb = GetComponent<PlayerBehaviours>();
            PlayerData.pb = pb;
            PlayerData.playerCols = new Collider2D[2];
            PlayerData.playerCols[0] = col;
            PlayerData.playerCols[1] = cc;
        }
        colliderSize = cc.offset;
    }

    
    
    void FixedUpdate() {
        if (stun) {
            return;
        }
        
        if (isPlayer) {
            if (pb.inMinigame) return;
            if (pb.hypnotizing && !iAmTarget) return;
            
            Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"),0);
            move = new Vector2(dir.x,dir.y);
            climbing = Input.GetAxis("Vertical") > 0;
        }
        
        
        
        if (move.x == 0){
            animator.Play("Idle");
        } else if (move.x != 0) { animator.Play("WalkCycle"); }
        
       
        
        SlopeCheck();
        GroundCheck();
        
        if (move.x > 0 && !m_FacingRight) { // NRC: flip flop por size.
            Flip();
        } else if (move.x < 0 && m_FacingRight) {
            Flip();
        }

        if (isGrounded && !isSlope) {
            move.Set(Speed*move.x,0.0f);
            rb.velocity = move;
            
        } else if(isGrounded && isSlope) {
            move.Set(Speed*normalPerp.x*-move.x, Speed*normalPerp.y*-move.x);
            rb.velocity = move;
            
        } else if(!isGrounded) {
            move.Set(Speed*move.x, rb.velocity.y);
            rb.velocity = move;
        }
        
        rb.MovePosition(rb.position+move*Speed*Time.fixedDeltaTime);
        move = Vector2.zero;
        
        if (inFly) {
            rb.AddForce(Vector2.down*500);
        }
    }

    public void SetMove(Vector2 dir) {
        move = dir;
        climbing = dir.y > 0;
    }
    
    private void Flip() {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        ParentTurn?.Invoke();
    }
    
    private void SlopeCheck()
    {
        Vector2 CheckPos = transform.position - new Vector3(0.0f, colliderSize.y/2);
        CheckVertical(CheckPos);
        CheckHorizontal(CheckPos);
    }
    
    private void CheckHorizontal(Vector2 CheckPos) {
        RaycastHit2D hitFront = Physics2D.Raycast(CheckPos, transform.right, checkDistance, whatIsGround);
        RaycastHit2D hitBack = Physics2D.Raycast(CheckPos, -transform.right, checkDistance, whatIsGround);
        if(hitFront) {
            isSlope = true;
            slopeSideAngle = Vector2.Angle(hitFront.normal, Vector2.up);
            
        } else if (hitBack) {
            isSlope = true;
            slopeSideAngle = Vector2.Angle(hitBack.normal, Vector2.up);
            
        } else {
            slopeSideAngle = 0.0f;
            isSlope = false;
        }
    }
    public void isStuned() {
        stun = true;
        StartCoroutine(Stunned());
    }

    IEnumerator Stunned() {
        var behaviors = GetComponent<GuardaBehaviors>();
        animator.Play("Stun");
        yield return new WaitUntil(() => !behaviors.stunned);
        stun = false;
    }
    
    
    private void CheckVertical(Vector2 CheckPos) {
        RaycastHit2D hit = Physics2D.Raycast(CheckPos, Vector2.down,checkDistance,whatIsGround);
        if (hit) {
            if (Physics2D.GetIgnoreCollision(hit.collider, col)) {
                
                return;
            }
            
            normalPerp = Vector2.Perpendicular(hit.normal).normalized;
            downAngle = Vector2.Angle(hit.normal,Vector2.up);

            if (downAngle != SlopeDownOld) {
                isSlope = true;
            }
            
            SlopeDownOld = downAngle;
            isGrounded = true;

            if (hit.collider.CompareTag("Stairs")) { // NRC: compara tag do collider que ta em baixo do player 
                oldStair = hit.collider;
            } else if(oldStair != null) {
                IgnoreCollision(oldStair);
                oldStair = null;
            }
            
            Debug.DrawRay(hit.point, normalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }

    public void SetOldStair(Collider2D stair) {
        oldStair = stair;
    }
    
   void GroundCheck() {
       inFly = true;
       Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCol.position,0.2f,whatIsGround);
       
       for (int i = 0; i < colliders.Length; i++) { // todo isso aqui ta um perigo meuuuuuuuuuu AMIIIIIIGO
           if (Physics2D.GetIgnoreCollision(colliders[i], col)) {
               Collider2D[] copy = new Collider2D[colliders.Length - 1];
               int idx = 0;
               for (int j = 0; j < colliders.Length; j++) {
                   if(j == i) continue;
                   copy[idx++] = colliders[j];
               }
               colliders = copy;
           }
       }

       if(colliders.Length > 0 ) inFly = false;
   }

   public void IgnoreCollision(Collider2D collider, bool condition = true) {
       var mycols = GetComponents<Collider2D>();
       foreach (var _col in mycols) {
        Physics2D.IgnoreCollision(_col, collider, condition);
       }
   }

   public void Hypnotize(bool isIn) {
       pb = isIn ? PlayerData.pb : null;
       if (!isIn) {
           PlayerData.pb.target = null;
           PlayerData.pb.hypnotizing = false;
           PlayerBehaviours?.Invoke();
       }
       
       iAmTarget = isIn;
       isPlayer = isIn;
       tag = isIn ? "Player" : "NPC";
       GetComponent<GuardaBehaviors>().hypnotized = isIn;
       
   }
}
