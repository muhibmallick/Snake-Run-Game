using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class DeathScript : MonoBehaviour
{
    [SerializeField] private TMP_Text gameOverText;
    [Tooltip("What should be the game over text, displayed when the snake's score reaches 0")]
    [SerializeField] private string? intendedText;
    [Tooltip("Gameover panel to show after snake death")]
    [SerializeField] GameObject gameOverPanel;

    [Tooltip("When the snake dies, after how many seconds, the level should start again")]
    [SerializeField] float maxTime = 3f;

    private SnakeController snakeController;


    // getters
    public GameObject GameOverPanel() => this.gameOverPanel;




    // private members
    float timer;


    private void Start()
    {
        snakeController = GetComponent<SnakeController>();
        gameOverText.text = intendedText;
        gameOverText.gameObject.SetActive(false);
        gameOverPanel.SetActive(false);
    }


    private void Update()
    {
        WhatIfScoreIsZero();
    }

    private void WhatIfScoreIsZero()
    {
        bool scoreIsZero = snakeController.Score() <= 0 ? true : false;


        if (scoreIsZero)
        {
            // manage death requirements
            snakeController.canMove = false;
            gameOverPanel.SetActive(true);
        }
    }

    public void OnClickTryAgainBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
