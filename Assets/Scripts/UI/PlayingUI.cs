using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayingUI : MonoBehaviour
{
    private GameManager gameManager;

    [Header("References")]
    [SerializeField] private TMP_Text player1ScoreText;
    [SerializeField] private TMP_Text player2ScoreText;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        player1ScoreText.text = gameManager.Player1Score.ToString();
        player2ScoreText.text = gameManager.Player2Score.ToString();
    }
}
