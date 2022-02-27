using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject go;
    public static GameObject _go;
    // Update is called once per frame

    private void Awake() {
        go.SetActive(isPaused);
        _go = go;
    }

    void Update() {
    
        
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(isPaused) {
                Resume();
            }else {Pause();}
        }
    }
    public static void Resume()
    {
        _go.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    void Pause()
    {
        _go.SetActive(true);
        if (!PlayerData.pb.inMinigame) {
            Time.timeScale = 0.0f;
        }
        isPaused = true;
    }
    public void Quit(){Application.Quit();}
}
