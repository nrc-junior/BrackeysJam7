using UnityEngine;
using UnityEngine.Events;

public class Hide : MonoBehaviour
{
    public Player player;
    private void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.CompareTag("Player"))
        {   
            player.hidden = true;
            print("Escondido");
        }   
    }
    private void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.CompareTag("Player"))
        {
            player.hidden = false;
            print("Exposto");
        }   
    }
}
