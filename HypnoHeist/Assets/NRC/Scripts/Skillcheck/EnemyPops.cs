using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPops : MonoBehaviour {
    private void OnEnable() => SkillCheck.OnStart += CallRoutine;
    private void OnDisable() => SkillCheck.OnStart -= CallRoutine;
    void CallRoutine() => StartCoroutine(popAll());

    private SkillCheck popManager;
    private void Awake() => popManager = GetComponent<SkillCheck>();

    private List<BubbleElement> bubbles;
    IEnumerator popAll() {
        bubbles = new List<BubbleElement>(popManager.bubbles) ;
        float difficulty = PlayerData.GetBotVel();
        while (popManager.Playing) {
            yield return new WaitForSeconds(difficulty);
            EnemyPop();
        }
    }
    
    public void EnemyPop() {
        foreach (var i in bubbles) {
            if (i.side == Sides.bot) {
                i.Pop();
                bubbles.Remove(i);
                break;
            }
        }
    }
}
