using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private GameManager gameManager;

    [Header("References")]
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        restartButton.onClick.AddListener(RestartButton_OnClick);

        gameManager.OnStateChange += GameManager_OnStateChange;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void GameManager_OnStateChange(GameManager.State newState)
    {
        if(newState == GameManager.State.GameOver)
        {
            gameObject.SetActive(true);

            winnerText.text = gameManager.Player1Score > gameManager.Player2Score ? "Player 1 Wins!" : "Player 2 Wins!";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void RestartButton_OnClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveListener(RestartButton_OnClick);

        gameManager.OnStateChange -= GameManager_OnStateChange;
    }
}
