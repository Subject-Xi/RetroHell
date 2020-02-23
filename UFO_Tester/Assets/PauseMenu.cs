using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused;

    public GameObject PauseMenuUI;


    // Update is called once per frame
    void Update()
    {
        if (IsPaused)
        {
        	PauseMenuUI.SetActive(true);
        	Time.timeScale = 0f;
        	Debug.Log("Paused");
        }
        else
        {
        	PauseMenuUI.SetActive(false);
        	Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
        	IsPaused = !IsPaused;
        	Debug.Log("Escape key pressed");
        }

        if (Input.GetKey(KeyCode.M))
        {
        	Debug.Log("M key pressed");
        	LoadMenu();
        }
    }

    public void Resume()
    {
    	IsPaused = !IsPaused;
    }

  //  void Pause()
   // {
   // 	pauseMenuUI.SetActive(true);
    //	Time.timeScale = 0f;
    //	GameIsPaused = true;
   // }

    public void LoadMenu()
    {
    	Time.timeScale = 1f;
    	SceneManager.LoadScene("Start_Menu");
    	IsPaused = !IsPaused;
    }

    public void QuitGame()
    {
    	Debug.Log("Quitting game...");
    	Application.Quit();
    }
}
