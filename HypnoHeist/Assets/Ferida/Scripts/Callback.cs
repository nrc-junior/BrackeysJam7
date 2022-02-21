using UnityEngine;
using UnityEngine.Events;

public class Callback : MonoBehaviour
{
    [SerializeField] private UnityEvent triggerEventEnter; 
    [SerializeField] private UnityEvent triggerEventExit; 
    [SerializeField] private string objectTag = "Player";
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag(objectTag))
        {
            triggerEventEnter.Invoke();
        }   
    }
    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag(objectTag))
        {
            triggerEventExit.Invoke();
        }   
    }
}
