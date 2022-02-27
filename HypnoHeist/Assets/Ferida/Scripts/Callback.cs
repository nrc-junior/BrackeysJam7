using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Callback : MonoBehaviour
{
    [SerializeField] private UnityEvent triggerEventEnter; 
    [SerializeField] private UnityEvent triggerEventExit; 
    [SerializeField] private string objectTag = "Player";
    [SerializeField] private bool interactive;
    
    private PlayerBehaviours pb;    
    private bool canUse = true;
    private bool trig = false;

    private void OnEnable() => Movement.PlayerBehaviours += CleanPb;
    private void OnDisable() => Movement.PlayerBehaviours -= CleanPb;
    public void CleanPb() => pb = null;

    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag(objectTag)) {
            if (pb == null) pb = PlayerData.pb;
            print("entrou");
            if (interactive) {
                trig = true;
                return;
            }
            
            triggerEventEnter.Invoke();
        }   
    }
    private void FixedUpdate() {
        if (!interactive) return;
        if (!trig || !canUse || pb == null) return;
        if (!pb.isInteracting && interactive) return;
        StartCoroutine(UseInvoke());
    }

    IEnumerator UseInvoke() {
        canUse = false;
        triggerEventEnter.Invoke();
        yield return new WaitUntil(() => { if (pb != null) { return !pb.isInteracting; } return true; });
        canUse = true;

    }
    
    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag(objectTag)) {
            print("saiu");
            trig = false;
            triggerEventExit.Invoke();
            pb = null;
        }   
    }

}
