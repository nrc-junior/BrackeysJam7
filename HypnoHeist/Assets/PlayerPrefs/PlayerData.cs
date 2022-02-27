using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour {
    public static bool madeTutorial;
    public static bool playIntro = false;
    public static int defaultRng = 42;
    public static int savedRng ;

    public static int playTimes;
    static float[] botVelocity = {0.547f, 0.497f, 0.42f, .349f, 0.297f};

    public static float GetBotVel() =>  
        playTimes > botVelocity.Length - 1 ? botVelocity[botVelocity.Length - 1] : botVelocity[playTimes];
    
    static public Collider2D[] playerCols;
    static public PlayerBehaviours pb;
}
