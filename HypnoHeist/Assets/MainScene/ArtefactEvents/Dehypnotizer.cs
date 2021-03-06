using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dehypnotizer : MonoBehaviour {
    [SerializeField] private bool status = true;
    private Collider2D col;
    private void Start() {
     (col = GetComponent<Collider2D>()).enabled = status;
     GetComponentInChildren<Animator>().Play(status? "On":"Off");
    }

    public void Turn() {
        status = !status;
        col.enabled = status;
        GetComponentInChildren<Animator>().Play(status? "On":"Off");
        
    }

    public void Dehypnotize() {
        if(PlayerData.pb.hypnotizing) PlayerData.pb.Dehypnotize();
    }
    
}
