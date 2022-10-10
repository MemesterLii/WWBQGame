using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI finalScore;

    public void Show(int score)
    {
        Time.timeScale = 0;
        gameObject.SetActive(true);
        finalScore.text = score.ToString();
    }
    void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    void MainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
