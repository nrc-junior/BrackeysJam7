using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hipnotize : MonoBehaviour {
    
    SpriteRenderer enemyBox;
    Transform transform1;
    SpriteRenderer warnBox;
    private void Start() {
        enemyBox = GetComponent<SpriteRenderer>();
        warnBox = GetComponentsInChildren<SpriteRenderer>()[1];

        enemyBox.enabled = false;
        warnBox.enabled = false;
        
        transform1 = warnBox.transform;
        transform.parent.GetComponent<Movement>().ParentTurn += FlipText;
    }

    public void FlipText() {
        print("flip");
        var scale = transform1.localScale;
        transform1.localScale = new Vector3(-scale.x, scale.y, scale.z);
    } 

    public void DrawBoxes(bool condition) {
        if (PlayerData.pb.hypnotizing) {
            enemyBox.enabled = false;
            warnBox.enabled = false;
            PlayerData.pb.hypnosisTrigger = false;
            return;
        }
        enemyBox.enabled = condition;
        warnBox.enabled = condition;
        PlayerData.pb.hypnosisTrigger = condition;
    }
    
    public void Hypnotize() {
        if (!warnBox.enabled) return;
        PlayerData.pb.Hypnosis(transform.parent.GetComponent<GuardaBehaviors>());
        
    }
}
