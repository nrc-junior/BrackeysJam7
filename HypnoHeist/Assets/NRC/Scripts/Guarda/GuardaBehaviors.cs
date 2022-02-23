using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SyncPath {
    [FormerlySerializedAs("inPointWaitFor")] public int pointToWaitFor;
    public GuardaBehaviors guarda;
    public int guardaReachPoint;
}

public class GuardaBehaviors : MonoBehaviour {
    private CircleCollider2D legsCol;
    private BoxCollider2D torsoCol;
    private Controller control;
    
    public List<Vector2> patrolPoints = new List<Vector2>();

    public int startPatrolIndex = 0;
    private bool patroling;
    private bool inversePatrolling;
    private Vector2 patrolTarget;
    private int pathIndex;


    public List<SyncPath> syncPaths;
    private SyncPath waitingFor;
    
    bool reachedPoint;
    private bool duringSync;
    
    // Detection
    const int playerLayer = 1 << 3;
    
    void Awake() {
        control = GetComponent<Controller>();
        torsoCol = GetComponent<BoxCollider2D>();
        legsCol = GetComponent<CircleCollider2D>();
    }

    private void Start() {
        GuardaBehaviors[] guardas = FindObjectsOfType<GuardaBehaviors>();

        foreach (var guarda in guardas) {
            Physics2D.IgnoreCollision(guarda.legsCol, legsCol);
            Physics2D.IgnoreCollision(guarda.legsCol, torsoCol);
            Physics2D.IgnoreCollision(guarda.torsoCol, legsCol);
            Physics2D.IgnoreCollision(guarda.torsoCol, torsoCol);
        }

        
        if (patrolPoints.Count > 1) {
            transform.position =  patrolPoints[startPatrolIndex];
            pathIndex = startPatrolIndex;
            patroling = true;
            if (startPatrolIndex + 1 > patrolPoints.Count - 1) {
                print("start tracing back");
                inversePatrolling = true;
            };
            CalculateNextPatrolPoint();
        }
        
    }

    private void FixedUpdate() {
        Vector2 dir = control.m_FacingRight ? transform.right : -transform.right ;
        var hit = Physics2D.Raycast(transform.position, dir, 30, playerLayer);
        if (hit) {
            if (hit.collider.CompareTag("Player")) {
                print( name + " sees player");
            }
        }
        
        if (!patroling) return;
        Vector2 pos = transform.position;
        
        control.Move(Mathf.Sign(patrolTarget.x - pos.x ));

        if(Vector2.Distance(pos, patrolTarget) < 1) {
            reachedPoint = true;
            if (CanCyclePatrol()) {
                if(duringSync) return;
                CalculateNextPatrolPoint();
                
            }
        }        
    }

    private void CalculateNextPatrolPoint() {
        if (patrolPoints.Count <= 1) return;
        pathIndex = inversePatrolling ? pathIndex - 1 : pathIndex + 1;
        patrolTarget = patrolPoints[pathIndex];
        
        if(duringSync) return;
        reachedPoint = false;
    }
    
    private bool CanCyclePatrol() {
        if (patrolPoints.Count <= 1) return patroling =  false;
        
        if (pathIndex + 1 > patrolPoints.Count - 1) {
            inversePatrolling = true;            
        }else if (pathIndex - 1 < 0) {
            inversePatrolling = false;
        }


        if (!(syncPaths.Count > 0)) return true;
        
        if (waitingFor != null) {
            return waitingFor.guarda.reachedPoint;
        }
        
        foreach (var syncPoint in syncPaths) {
            
            if (syncPoint.pointToWaitFor == pathIndex) {
                waitingFor = syncPoint;
                StartCoroutine(AwaitGuardaReach());       
            }
        }
        return true;
    }


    bool InPosition() {
        return waitingFor.guarda.pathIndex == waitingFor.guardaReachPoint && waitingFor.guarda.reachedPoint;
    }

    IEnumerator AwaitGuardaReach() {
        duringSync = true;
        yield return new WaitUntil(() => reachedPoint);
        patroling = false;
        
        yield return new WaitUntil(() => InPosition());
        yield return new WaitForSeconds(0.3f);
        print(name +" sync with "+ waitingFor.guarda.name);
        duringSync = false;
        patroling = true;
        waitingFor = null;
        CalculateNextPatrolPoint();
    }
}
