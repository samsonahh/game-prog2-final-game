using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("References")]
    private Rigidbody rigidBody;

    [Header("Local Multiplayer Settings")]
    [SerializeField] private bool isPlayerOne = true;

    [Header("Push Settings")]
    [SerializeField] private float pushForce = 5f;
    public Action<Player, Vector3> OnTouchOtherPlayer = delegate { };

    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 7.5f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float friction = 5f;
    private Vector3 movementDirection;

    [Header("Stun Settings")]
    [SerializeField] private float stunDuration = 1f;

    [Header("Death Settings")]
    [SerializeField] private float deathDuration = 1f;

    #region State Machine
    public enum State
    {
        Idle,
        Move,
        Stunned,
        Dead
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

    private void OnExitState(State state)
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

    private void UpdateState(State state)
    {
        switch (state)
        {
            case State.Idle:
                HandleMovementInput();

                rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, Vector3.zero, friction * Time.deltaTime);

                if (movementDirection != Vector3.zero)
                {
                    ChangeState(State.Move);
                    return;
                }

                break;
            case State.Move:
                HandleMovementInput();

                //if(movementDirection.x == 0) rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, new Vector3(0, 0, rigidBody.velocity.z), friction * Time.deltaTime);
                //if(movementDirection.z == 0) rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, new Vector3(rigidBody.velocity.x, 0, 0), friction * Time.deltaTime);

                rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);

                if (movementDirection == Vector3.zero)
                {
                    ChangeState(State.Idle);
                    return;
                }

                break;
            case State.Stunned:
                break;
            case State.Dead:
                break;
            default:
                break;
        }
    }

    private void FixedUpdateState(State state)
    {
        switch (state)
        {
            case State.Idle:
                
                break;
            case State.Move:
                ApplyMovement();
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

    private void Start()
    {
        ChangeState(State.Idle, true);
    }

    private void Update()
    {
        UpdateState(CurrentState);
    }

    private void FixedUpdate()
    {
        FixedUpdateState(CurrentState);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out Player otherPlayer))
        {
            Vector3 directionToOtherPlayer = (otherPlayer.transform.position - transform.position).normalized;

            otherPlayer.Push(directionToOtherPlayer, pushForce);

            OnTouchOtherPlayer?.Invoke(otherPlayer, collision.contacts[0].point);
        }
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

    private void ApplyMovement()
    {
        float angle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + Camera.main.transform.rotation.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, angle, 0f);
        Vector3 targetForwardDirection = targetRotation * Vector3.forward;

        rigidBody.AddForce(moveForce * targetForwardDirection);
    }

    /// <summary>
    /// Applies a push force to the player in the specified direction.
    /// </summary>
    /// <param name="direction">The direction in which to apply the force.</param>
    /// <param name="force">The magnitude of the force to apply.</param>
    public void Push(Vector3 direction, float force)
    {
        rigidBody.AddForce(force * direction.normalized, ForceMode.Impulse);
    }
}
