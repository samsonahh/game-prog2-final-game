using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("References")]
    private Rigidbody rigidBody;

    [Header("Local Multiplayer Settings")]
    [SerializeField] private bool isPlayerOne = true;

    [Header("Movement Settings")]
    [SerializeField] private float baseMoveSpeed = 3f;
    private float moveSpeedModifier = 1f;
    private float totalMoveSpeed => baseMoveSpeed * moveSpeedModifier;
    private Vector3 movementDirection;

    private bool isMoving => movementDirection != Vector3.zero;

    #region State Machine
    public enum State
    {
        Idle,
        Move,
        Stunned,
        Dead
    }
    
    public State CurrentState { get; private set; }

    private void ChangeState(State newState)
    {
        OnExitState(CurrentState);
        newState = CurrentState;
        OnEnterState(CurrentState);
    }

    private void OnEnterState(State state)
    {

    }

    private void OnExitState(State state)
    {

    }

    private void UpdateState(State state)
    {

    }

    private void FixedUpdateState(State state)
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Move:
                break;
            case State.Stunned:
                break;
            case State.Dead:
                break;
            default:
                break;
        }
    }
    #endregion

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateState(CurrentState);
    }

    private void FixedUpdate()
    {
        FixedUpdateState(CurrentState);
    }

    private void HandleMovementInput()
    {
        Vector3 direction = Vector3.zero;

        if (isPlayerOne) // adjusts movementDirection based on WASD inputs
        {
            if(Input.GetKey(KeyCode.W)) direction = new Vector3(direction.x, 0f, 1f);
            if(Input.GetKey(KeyCode.S)) direction = new Vector3(direction.x, 0f, -1f);

            if(Input.GetKey(KeyCode.D)) direction = new Vector3(1f, 0f, direction.z);
            if(Input.GetKey(KeyCode.A)) direction = new Vector3(-1f, 0f, direction.z);

            if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) direction = new Vector3(direction.x, 0f, 0f);
            if(Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A)) direction = new Vector3(0f, 0f, direction.z);
        }
        else // adjusts movementDirection based on arrow key inputs
        {
            if (Input.GetKey(KeyCode.UpArrow)) direction = new Vector3(direction.x, 0f, 1f);
            if (Input.GetKey(KeyCode.DownArrow)) direction = new Vector3(direction.x, 0f, -1f);

            if (Input.GetKey(KeyCode.RightArrow)) direction = new Vector3(1f, 0f, direction.z);
            if (Input.GetKey(KeyCode.LeftArrow)) direction = new Vector3(-1f, 0f, direction.z);

            if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow)) direction = new Vector3(direction.x, 0f, 0f);
            if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.LeftArrow)) direction = new Vector3(0f, 0f, direction.z);
        }

        movementDirection = direction.normalized;
    }

    private void Move()
    {
        rigidBody.MovePosition(transform.position + Time.fixedDeltaTime * totalMoveSpeed * movementDirection);
    }
}
