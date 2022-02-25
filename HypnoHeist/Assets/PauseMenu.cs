using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isPaused = false;
    public GameObject go;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPaused)
            {
                Resume();
            }else {Pause();}
        }
    }
    public void Resume()
    {
        go.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    void Pause()
    {
        go.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }
    public void Load(){SceneManager.LoadScene("Menu");}
    public void Quit(){Application.Quit();}
}
