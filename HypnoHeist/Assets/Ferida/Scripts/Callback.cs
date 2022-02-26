using System;
using UnityEngine;
using UnityEngine.Events;

public class Callback : MonoBehaviour
{
    [SerializeField] private UnityEvent triggerEventEnter; 
    [SerializeField] private UnityEvent triggerEventExit; 
    [SerializeField] private string objectTag = "Player";
    [SerializeField] private bool interactive;

    private PlayerBehaviours pb;
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag(objectTag))
        {
            if (pb == null) pb = other.GetComponent<PlayerBehaviours>();
            if(interactive) return;
            
            triggerEventEnter.Invoke();
        }   
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (pb == null || !interactive) return;
        if(!pb.isInteracting && interactive) return;
        pb.isInteracting = false;
        triggerEventEnter.Invoke();
    }

    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag(objectTag))
        {
            triggerEventExit.Invoke();
            pb = null;
        }   
    }
}
