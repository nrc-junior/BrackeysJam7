using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum  Animations {
    fadeIn,
    fadeOut,
    death,
}

public class DarkScreen : MonoBehaviour {
    private static Animator animator;
    private static Dictionary<Animations, string> clips = new Dictionary<Animations, string>();
    void Awake() {
        animator = GetComponent<Animator>();
        clips[Animations.fadeIn] = "DarkScreenIn";
        clips[Animations.fadeOut] = "DarkScreenOut";
        clips[Animations.death] = "DeathScreen";
        
    }

    public static void Play(Animations clip) {
        animator.Play(clips[clip]);
    }

}
