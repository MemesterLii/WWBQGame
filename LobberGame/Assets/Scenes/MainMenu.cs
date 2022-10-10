using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Awake()
    {
        Cursor.visible = true;
    }

    void PlayGame()
    {
        SceneManager.LoadScene(/*SceneManager.GetActiveScene().buildIndex + 1*/"Game");
    }
}
