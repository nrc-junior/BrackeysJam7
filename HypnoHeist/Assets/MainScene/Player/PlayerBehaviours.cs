using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBehaviours : MonoBehaviour {
    private bool interacting;
    private bool hide;
    public bool inMinigame;
    public bool hypnosisTrigger; 
    [SerializeField] private GameObject miniGame;
    private Animator animatorMiniGame;
    private GuardaBehaviors adversary;
    
    public delegate void HypnosisEvent();
    public static event HypnosisEvent onHypnosis;
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q) && hypnotizing) {
           Dehypnotize();
        }
    
        if (Input.GetKeyDown(KeyCode.E) || (hypnosisTrigger && Input.GetKeyDown(KeyCode.Q))) {
            interacting = true;
        }else if((Input.GetKeyUp(KeyCode.E) || (hypnosisTrigger && Input.GetKeyUp(KeyCode.Q)))) {
            interacting = false;
        }
    }

    public void Dehypnotize() {
        target.GetComponent<Movement>().Hypnotize(false);
        hypnotizing = false;
        onHypnosis?.Invoke();
    }
    
    public void SetHiding(bool v) {
        if(hypnotizing) return;
        
        hide = v;
        
        var img = GetComponent<SpriteRenderer>();
        img.color = v ? Color.gray : Color.white;
    }
    
    public bool isHide => hide;
    
    public bool isInteracting {
        get => interacting;
        set => interacting = value;
    }

    public void StartMinigame(GuardaBehaviors guarda) {
      adversary = guarda;
      miniGame.SetActive(true);
      miniGame.GetComponent<SkillCheck>().StartMinigame();
    }

    public void EndMiniGame(Sides winner) {
        PlayerData.playTimes++;
        miniGame.SetActive(false);
        inMinigame = false;
        
        if (winner == Sides.bot) {
            Die();
        }else {
            adversary.Stun();
        }
    }

    public bool hypnotizing;
    public GuardaBehaviors target;
    public void Hypnosis(GuardaBehaviors target) {
        this.target = target;
        hypnotizing = true;
        
        onHypnosis?.Invoke();
        this.target.GetComponent<Movement>().Hypnotize(true);
        print("hypnotizing " + target);
    }
    
    public void Die() {
        DarkScreen.Play(Animations.death);
        inMinigame = true;
        StartCoroutine(RestartLevel());
    }

    public void Credits(){
        DarkScreen.Play(Animations.credits);
        StartCoroutine(End());
    }

    IEnumerator End() {
        yield return new WaitForSeconds(18);
        Application.Quit();
    }
    
    IEnumerator RestartLevel() {
        yield return new WaitForSeconds(2);
        LevelManager.Restart();
    }
}
