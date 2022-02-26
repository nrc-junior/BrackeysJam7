using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviours : MonoBehaviour {
    public void Die() {
        print("player died");
    }

    private bool interacting;

    public bool isInteracting
    {
        get => interacting;
        set => interacting = value;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) {
            interacting = true;
        }else if (Input.GetKeyUp(KeyCode.E)) {
            interacting = false;
        }
    }
    
}
