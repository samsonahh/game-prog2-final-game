using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class IntermissionUI : MonoBehaviour
{
    private GameManager gameManager;

    [Header("References")]
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text timerText;

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
        if(newState == GameManager.State.Intermission)
        {
            gameObject.SetActive(true);

            roundText.text = $"Round {gameManager.CurrentRound + 1}/{gameManager.Rounds}";
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if(gameManager.IntermissionDuration - (int)gameManager.IntermissionTimer > 0)
            timerText.text = $"{gameManager.IntermissionDuration - (int)gameManager.IntermissionTimer}"; 
    }

    private void OnDestroy()
    {
        gameManager.OnStateChange -= GameManager_OnStateChange;
    }
}
