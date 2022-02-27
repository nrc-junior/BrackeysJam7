using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum Sides {
  player,
  bot
}

public class SkillCheck : MonoBehaviour {
  [SerializeField] RectTransform left;
  [SerializeField] RectTransform right;
  [SerializeField] GameObject bubble;
  
  [SerializeField] TextMeshProUGUI round;
  [SerializeField] RectTransform brainL;
  [SerializeField] RectTransform brainR;
  [SerializeField] RectTransform attackLine;
  [SerializeField] ParticleSystem particles;
  public static RectTransform _AttackEffect;
  
  public static Color PURPLE = new Color(.35f,0,1,0.4f);
  public static Color ORANGE = new Color(1,0.35f,0,0.4f);
  
  Vector2 l_canvasWidth;
  Vector2 l_CanvasHeight;
  
  Vector2 r_canvasWidth;
  Vector2 r_CanvasHeight;

  private Vector2 windowRes;
  
  private int roundIndex;
  private int[] waves = {4, 6, 8};

  public static SkillCheck _;
  [HideInInspector] public List<BubbleElement> bubbles = new List<BubbleElement>();
  private int playerPops;
  private int enemyPops;

  public delegate void RoundBegins();
  public static event RoundBegins OnStart;
  
  // Desafio
  //private bool benchmark = false;
  private float[] benchTimes;
  private int benchIdx = 0;
  public InputField benchmarkTest;
  
  [SerializeField] private Slider score;
  [SerializeField] GameObject[] tutorial;
  private void Awake() {
    var resolution = Screen.currentResolution;
    windowRes = new Vector2(resolution.width, resolution.height);
    GetComponent<CanvasScaler>().referenceResolution = windowRes;
  }

  public void StartMinigame() {
    _ = this;

    GetComponent<Animator>().Play("MinigameFadeIn");    
    _AttackEffect = attackLine;
  
    l_canvasWidth = new Vector2(left.anchorMin.x, left.anchorMax.x);
    l_CanvasHeight = new Vector2(left.anchorMin.y, left.anchorMax.y);
    
    r_canvasWidth = new Vector2(right.anchorMin.x, right.anchorMax.x);
    r_CanvasHeight = new Vector2(right.anchorMin.y, right.anchorMax.y);

    int bubbleCount = waves[roundIndex <= 2 ? roundIndex : 2];
    playerPops = bubbleCount;
    enemyPops = bubbleCount;
    
    if (!PlayerData.madeTutorial) {
      PlayerData.savedRng = (int)Random.value;
      Random.InitState(PlayerData.defaultRng);
      StartCoroutine(ShowTutorial());
    }
    
    BeginRound(bubbleCount);
  }

  IEnumerator ShowTutorial() {
    yield return new WaitForSeconds(.6f);

    while (Time.timeScale > 0.1f) {
      if (Time.timeScale - 0.15f <= 0.1f) Time.timeScale = 0.0f;
      
      Time.timeScale -= 0.1f;
      yield return new WaitForSecondsRealtime(0.01f);
    }

    foreach (var o in tutorial) {
      o.SetActive(true);
    }

  }

  public void BeginRound(int count) {
    round.text = "round "+ (++roundIndex).ToString();
    bubbles = new List<BubbleElement>();
    
    benchTimes = new float[count+1];
    benchTimes[0] = Time.time;
    benchIdx = 0;
    
    for (int i = 0; i < count; i++) {
      SpawnElement(Sides.player);
      SpawnElement(Sides.bot);
    }

    Playing = true;
    OnStart?.Invoke();
  }
  
  void SpawnElement(Sides owner) {
    BubbleElement bubbleInstance = Instantiate(bubble, transform).GetComponent<BubbleElement>();
    
    bubbleInstance.Setup(
      owner == Sides.player ? 
        l_canvasWidth : r_canvasWidth,
      owner == Sides.player ?
        l_CanvasHeight : r_CanvasHeight , windowRes, 
      owner == Sides.player ? brainR : brainL,
      Instantiate(particles, transform), owner,
      owner == Sides.player ? ORANGE : PURPLE );
    bubbles.Add(bubbleInstance);
  }

  public void Popped(Sides side) {

    if (side == Sides.player) {
      benchTimes[++benchIdx] = Time.time;

      if (--playerPops == 0 && Playing) { // PLAYER Wins 
        if (!PlayerData.madeTutorial) {
          PlayerData.madeTutorial = true;
          Random.InitState(PlayerData.savedRng);
          Time.timeScale = 1f;
          
          foreach (var o in tutorial) {
            o.SetActive(false);
          }
        }
        
        Playing = false;
        float sum = 0;
        for (int i = 0; i < benchTimes.Length-1; i++) {
          sum += benchTimes[i + 1] - benchTimes[i];
        }
        // todo: sum / (benchTimes.Length-1) + "+" ; // player speed combo
        
        StartCoroutine(Attack(Sides.player)); // Attack and VFX 
      }
    } else if (--enemyPops == 0 && Playing) { // bot wins
      
        Playing = false;
        StartCoroutine(Attack(Sides.bot)); // Attack and VFX 
    }
  }

  //VFX
  [HideInInspector] public bool duringAttack;
  
  IEnumerator Attack(Sides side) {
    
    foreach (var i in bubbles) {
      if(i.side == side)
        i.SetExternalForces();
    }

    foreach (var i in bubbles) {
      if (i.side == side) {
        i.IncreaseParticles(100);
        yield return new WaitForSeconds(.2f);
      }
    }
    yield return new WaitForSeconds(1);
    
    foreach (var i in bubbles) {
      if (i.side == side) {
        i.Attack(); 
        yield return new WaitForSeconds(.2f);
        
        float dmg = side == Sides.player ? .1f : -.1f; //damage
        score.value += dmg * (roundIndex/2f);
        i.IncreaseParticles(0);
      }
    }

    yield return new WaitForSeconds(1);
    
    foreach (var i in bubbles) {
      i.RemoveBubble();
    } 
    yield return new WaitForSeconds(2);
    if (score.value >= score.maxValue || score.value <= score.minValue ) {
      GetComponent<Animator>().Play("MinigameFadeOut");
      yield return new WaitForSeconds(1);
      ended();
    } else {
      RoundEnded();
    }
  }

  void RoundEnded() {
    int bubbleCount = waves[roundIndex <= 2 ? roundIndex : 2];
    playerPops = bubbleCount;
    enemyPops = bubbleCount;
    BeginRound(bubbleCount);
  }

  void ended() {
    bool win = score.value >= score.maxValue;    
    
    roundIndex = 0;
    int bubbleCount = waves[roundIndex <= 2 ? roundIndex : 2];
    playerPops = bubbleCount;
    enemyPops = bubbleCount;
    score.value = score.maxValue / 2;
    PlayerData.pb.EndMiniGame(win ? Sides.player : Sides.bot);
    
    
  }

  /// 
  /// Bot Controls ↓ 
  ///
  private bool roundStarted;
  public bool Playing {
    get => roundStarted;
    set => roundStarted = value;
  }
  
}

