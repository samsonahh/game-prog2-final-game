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

    private Player[] players;

    [field: Header("Scores")]
    [field: SerializeField] public int Rounds { get; private set; } = 5;
    public int CurrentRound { get; private set; }
    public int[] Scores { get; private set; }

    [field: Header("Intermission Settings")]
    [field: SerializeField] public int IntermissionDuration { get; private set; } = 3;
    public float IntermissionTimer { get; private set; } = 0f;

    [field: Header("Intermission Settings")]
    [field: SerializeField] public int RoundEndDuration { get; private set; } = 1;
    public float RoundEndTimer { get; private set; } = 0f;

    #region State Machine
    public enum State
    {
        Intermission,
        Playing,
        RoundEnd,
        GameOver
    }

    public State CurrentState { get; private set; }

    private void ChangeState(State newState, bool willForceChange = false)
    {
        if (!willForceChange && CurrentState == newState) return;

        OnExitState(CurrentState);
        CurrentState = newState;
        OnEnterState(CurrentState);
    }

    private void OnEnterState(State state)
    {
        switch (state)
        {
            case State.Intermission:

                Time.timeScale = 0f;

                IntermissionTimer = 0;

                EnablePlayers();
                break;
            case State.Playing:

                Time.timeScale = 1f;

                GiveBombToRandomPlayer();
                break;
            case State.RoundEnd:

                Time.timeScale = 0f;

                break;
            case State.GameOver:

                Time.timeScale = 0f;

                RoundEndTimer = 0f;

                break;
            default:
                break;
        }

        Debug.Log(state.ToString());
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
                IntermissionTimer += Time.unscaledDeltaTime;
                Debug.Log(IntermissionTimer);

                if(IntermissionTimer >= IntermissionDuration)
                {
                    ChangeState(State.Playing);
                    return;
                }
                break;
            case State.Playing:
                break;
            case State.RoundEnd:

                RoundEndTimer += Time.unscaledDeltaTime;
                Debug.Log(IntermissionTimer);

                if (RoundEndTimer >= RoundEndDuration)
                {
                    ChangeState(State.Intermission);
                    return;
                }
                break;
            case State.GameOver:
                break;
            default:
                break;
        }
    }
    #endregion

    private void Awake()
    {
        players = FindObjectsOfType<Player>();    
        Scores = new int[players.Length];
    }

    private void Start()
    {
        ChangeState(State.Intermission, true);
    }

    private void Update()
    {
        UpdateState(CurrentState);
    }

    private void GiveBombToRandomPlayer()
    {
        int randomIndex = UnityEngine.Random.Range(0, players.Length);
        Player randomPlayer = players[randomIndex];

        bomb.transform.position = randomPlayer.transform.position;
        bomb.gameObject.SetActive(true);
        bomb.Init(randomPlayer);
    }

    public void OnBombExplode(Player victim)
    {
        Scores[Array.IndexOf(players, victim)]++;
        CurrentRound++;

        if(CurrentRound >= Rounds)
        {
            ChangeState(State.GameOver);
        }
        else
        {
            ChangeState(State.RoundEnd);
        }
    }

    private void EnablePlayers()
    {
        for(int i = 0; i < players.Length; i++)
        {
            players[i].gameObject.SetActive(true);
        }
    }
}
