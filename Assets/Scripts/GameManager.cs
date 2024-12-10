using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Bomb bomb;

    [Header("Player Settings")]
    [SerializeField] private Player player1;
    [SerializeField] private Player player2;
    [SerializeField] private float playFieldWidth = 5f;
    [SerializeField] private float playFieldHeight = 5f;
    public int CurrentRoundWinner { get; private set; }

    [field: Header("Scores")]
    [field: SerializeField] public int Rounds { get; private set; } = 5;
    public int CurrentRound { get; private set; }
    public int Player1Score { get; private set; }
    public int Player2Score { get; private set; }

    [field: Header("Intermission Settings")]
    [field: SerializeField] public int IntermissionDuration { get; private set; } = 3;
    [field: SerializeField] public float IntermissionTimer { get; private set; } = 0f;
    private bool isFirstFrameOver = false;

    [field: Header("Round End Settings")]
    [field: SerializeField] public int RoundEndDuration { get; private set; } = 1;
    [field: SerializeField] public float RoundEndTimer { get; private set; } = 0f;

    #region State Machine
    public enum State
    {
        Intermission,
        Playing,
        RoundEnd,
        GameOver
    }

    [field: SerializeField] public State CurrentState { get; private set; }
    public Action<State> OnStateChange = delegate { };

    private void ChangeState(State newState, bool willForceChange = false)
    {
        if (!willForceChange && CurrentState == newState) return;

        OnExitState(CurrentState);
        CurrentState = newState;
        OnEnterState(CurrentState);

        OnStateChange?.Invoke(CurrentState);
    }

    private void OnEnterState(State state)
    {
        switch (state)
        {
            case State.Intermission:
                IntermissionTimer = 0;
                Time.timeScale = 0f;
                ResetPlayers();
                break;
            case State.Playing:
                Time.timeScale = 1f;
                GiveBombToRandomPlayer();
                break;
            case State.RoundEnd:
                RoundEndTimer = 0f;
                Time.timeScale = 0f;
                break;
            case State.GameOver:
                Time.timeScale = 0f;
                break;
            default:
                break;
        }
    }

    private void OnExitState(State state)
    {
        switch (state)
        {
            case State.Intermission:
                break;
            case State.Playing:
                break;
            case State.RoundEnd:
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }

    private void UpdateState(State state)
    {
        switch (state)
        {
            case State.Intermission:
                // To prevent unscaled delta time from bugging out the first frame
                if (!isFirstFrameOver)
                {
                    isFirstFrameOver = true;
                    return;
                }

                if (IntermissionTimer >= IntermissionDuration)
                {
                    ChangeState(State.Playing);
                    return;
                }

                IntermissionTimer += Time.unscaledDeltaTime;
                break;
            case State.Playing:
                break;
            case State.RoundEnd:
                if (RoundEndTimer >= RoundEndDuration)
                {
                    if (CurrentRound >= Rounds)
                    {
                        ChangeState(State.GameOver);
                    }
                    else
                    {
                        ChangeState(State.Intermission);
                    }
                    return;
                }

                RoundEndTimer += Time.unscaledDeltaTime;
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }
    #endregion

    private void Start()
    {
        ChangeState(State.Intermission, true);
    }

    private void Update()
    {
        UpdateState(CurrentState);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(playFieldWidth * 2, 0.1f, playFieldHeight * 2));
    }

    private void GiveBombToRandomPlayer()
    {
        int randomIndex = UnityEngine.Random.Range(0, 2);
        Player randomPlayer = randomIndex == 0 ? player1 : player2;

        bomb.transform.position = randomPlayer.transform.position;
        bomb.gameObject.SetActive(true);
        bomb.Init(randomPlayer);
    }

    public void OnBombExplode(Player victim)
    {
        if(victim == player1)
        {
            Player2Score++;
            CurrentRoundWinner = 2;
        }
        else
        {
            Player1Score++;
            CurrentRoundWinner = 1;
        }
        CurrentRound++;

        ChangeState(State.RoundEnd);
    }

    private void ResetPlayers()
    {
        player1.transform.position = GetRandomPosition();
        player2.transform.position = GetRandomPosition();

        player1.gameObject.SetActive(true);
        player2.gameObject.SetActive(true);
    }

    private Vector3 GetRandomPosition()
    {
       return new Vector3(UnityEngine.Random.Range(-playFieldWidth, playFieldWidth), 0f, UnityEngine.Random.Range(-playFieldHeight, playFieldHeight));
    }
}
