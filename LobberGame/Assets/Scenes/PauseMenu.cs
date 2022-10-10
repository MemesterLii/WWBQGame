using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gamePaused = false;

    [SerializeField] GameObject PauseMenuUI;
    [SerializeField] GameObject crosshair;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;

        crosshair.SetActive(true);
        Cursor.visible = false;
    }

    void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        gamePaused = true;

        crosshair.SetActive(false);
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
