using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SyncPath {
    public GuardaBehaviors guarda;
    public int mePoint;
    public int youPoint;
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
    
    public bool ready;
    
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

        transform.position =  patrolPoints[startPatrolIndex];
        pathIndex = startPatrolIndex;
        
        if (patrolPoints.Count > 1) {
            patroling = true;
            if (startPatrolIndex + 1 > patrolPoints.Count - 1) {
                print("start tracing back");
                inversePatrolling = true;
            };
            CalculateNextPatrolPoint();
        }
    }

    private void FixedUpdate() {
        if (!patroling) return;
        Vector2 pos = transform.position;
        
        control.Move(Mathf.Sign(patrolTarget.x - pos.x ));

        if(Vector2.Distance(pos, patrolTarget) < 1) {
            if (CanCyclePatrol()) {
                CalculateNextPatrolPoint();
            }
        }        
    }

    private void CalculateNextPatrolPoint() {
        pathIndex = inversePatrolling ? pathIndex - 1 : pathIndex + 1;
        patrolTarget = patrolPoints[pathIndex];
    }
    
    private bool CanCyclePatrol() {
        
        
        if (pathIndex + 1 > patrolPoints.Count - 1) {
            inversePatrolling = true;            
        }else if (pathIndex - 1 < 0) {
            inversePatrolling = false;
        }


        if (!(syncPaths.Count > 0)) return true;

        foreach (var waitGuarda in syncPaths) {
            if (waitGuarda.youPoint == pathIndex) {
                waitingFor = waitGuarda;
                StartCoroutine(AwaitGuardaReach());
            }
            
        }
        return true;
    }


    bool InPosition() {
        return waitingFor.guarda.pathIndex == waitingFor.mePoint;
    }

    IEnumerator AwaitGuardaReach() {
        print(name + "Stop patrolling");
        patroling = false;
        yield return new WaitUntil(() => InPosition());
        patroling = true;
        print(name + "Back patrolling");
    }
}
