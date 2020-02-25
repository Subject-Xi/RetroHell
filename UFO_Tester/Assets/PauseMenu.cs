using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
        	if (GameIsPaused)
        	{
        		Pause();
        	}
        	else
        	{
        		Resume();
        	}
        }
    }

    public void Resume()
    {
    	pauseMenuUI.SetActive(false);
    	Time.timeScale = 1f;
    	GameIsPaused = false;
    }

    void Pause()
    {
    	pauseMenuUI.SetActive(true);
    	Time.timeScale = 0f;
    	GameIsPaused = true;
    }

    public void LoadMenu()
    {
    	Time.timeScale = 1f;
    	SceneManager.LoadScene("Start_Menu");
    }

    public void QuitGame()
    {
    	Debug.Log("Quitting game...");
    	Application.Quit();
    }
}
