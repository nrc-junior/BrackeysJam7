using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairCheck : MonoBehaviour {
    [SerializeField] private Collider2D stair;
    [SerializeField] private bool triggerDown;
    Movement walker;
    
    private bool updated;

    private void OnTriggerEnter2D(Collider2D col) {
        if (!triggerDown) {
            updated = false;
            walker = null;
            return;
        }
        
        if (col.CompareTag("Player") || col.CompareTag("NPC")) {
            if (walker == null) walker = col.GetComponent<Movement>();
            walker.IgnoreCollision(stair, false);
        }
    }

    void OnTriggerStay2D(Collider2D col) {
        if (walker != null && updated) {
            updated = false;
            return;
        }

        if (col.CompareTag("Player") || col.CompareTag("NPC")) {
            if (walker == null) walker = col.GetComponent<Movement>();
            if (triggerDown) {
                walker.IgnoreCollision(stair, false);
                updated = true;
                return;
            }
            
            if (walker.climbing) {
                walker.IgnoreCollision(stair, false);
                updated = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        updated = false;
        walker = null;
    }
}
