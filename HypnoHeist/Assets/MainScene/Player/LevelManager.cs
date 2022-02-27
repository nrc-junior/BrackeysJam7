using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    private static int currentLevel;

    private void Awake() {
        currentLevel = SceneManager.GetActiveScene().buildIndex;
    }

    public static void LoadLevel(int level)
    {
        PlayerData.playTimes = 0;
        currentLevel = level;
        SceneManager.LoadScene(level);
    }

    public static void Restart() {
        PlayerData.playTimes = 0;
        SceneManager.LoadScene(currentLevel);
    }
}
