using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum  Animations {
    fadeIn,
    fadeOut,
    death,
    credits,
    intro,
}

public class DarkScreen : MonoBehaviour {
    private static Animator animator;
    private static Dictionary<Animations, string> clips = new Dictionary<Animations, string>();
    void Awake() {
        animator = GetComponent<Animator>();
        clips[Animations.fadeIn] = "DarkScreenIn";
        clips[Animations.fadeOut] = "DarkScreenOut";
        clips[Animations.death] = "DeathScreen";
        clips[Animations.credits] = "Credits";
        clips[Animations.intro] = "Intro";

        if (!PlayerData.playIntro) {
            PlayerData.playIntro = true;
            StartCoroutine(Intro());
            return;
        }
        
        Play(Animations.fadeOut);
        
    }

    IEnumerator Intro() {
        Play(Animations.intro);
        yield return new WaitForSeconds(30);
        Play(Animations.fadeOut);
    }

    public static void Play(Animations clip) {
        animator.Play(clips[clip]);
    }

}
