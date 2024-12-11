using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundEndUI : MonoBehaviour
{
    private GameManager gameManager;

    [Header("References")]
    [SerializeField] private TMP_Text winnerText;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        gameManager.OnStateChange += GameManager_OnStateChange;
    }

    private void Start()
    {
        
    }

    private void GameManager_OnStateChange(GameManager.State newState)
    {
        if (newState == GameManager.State.RoundEnd)
        {
            gameObject.SetActive(true);

            string winner = gameManager.CurrentRoundWinner == 1 ? "Player 1" : "Player 2";
            winnerText.text = $"{winner} Stands";
            winnerText.color = gameManager.CurrentRoundWinner == 1 ? gameManager.Player1Color : gameManager.Player2Color;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        gameManager.OnStateChange -= GameManager_OnStateChange;
    }
}
