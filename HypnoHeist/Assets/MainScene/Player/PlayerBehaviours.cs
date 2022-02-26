using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviours : MonoBehaviour {
    public void Die() {
        print("player died");
    }

    private bool interacting;
    public bool isInteracting {
        get => interacting;
        set => interacting = value;
    }
    
    private bool hide;
    public bool isHide => hide;
    public void SetHiding(bool v) {
        hide = v;
        
        var img = GetComponent<SpriteRenderer>();
        img.color = v ? Color.gray : Color.white;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            interacting = true;
        }else if (Input.GetKeyUp(KeyCode.E)) {
            interacting = false;
        }
    }
    
}
