using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("References")]
    private Rigidbody rigidBody;

    [Header("Local Multiplayer Settings")]
    [SerializeField] private bool isPlayerOne = true;

    [Header("Snowball Growth Settings")]
    [SerializeField] private float snowballGrowthSpeed = 0.5f;
    private float currentScale;
    private float startingScale;

    [Header("Push Settings")]
    [SerializeField] private float pushDamagePercent = 10f;
    [SerializeField] private float basePushForce = 5f;
    [SerializeField] private float pushForceLinearGrowth = 1f;

    [Header("Movement Settings")]
    [SerializeField] private float baseMoveSpeed = 3f;
    private float moveSpeedModifier = 1f;
    private float totalMoveSpeed => baseMoveSpeed * moveSpeedModifier;
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
    
    [field: SerializeField] public State CurrentState { get; private set; }

    private void ChangeState(State newState)
    {
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

                if (movementDirection != Vector3.zero)
                {
                    ChangeState(State.Move);
                    return;
                }

                break;
            case State.Move:
                HandleMovementInput();

                if(movementDirection == Vector3.zero)
                {
                    ChangeState(State.Idle);
                    return;
                }

                currentScale += snowballGrowthSpeed * Time.deltaTime;

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
        currentScale = transform.localScale.x;
        startingScale = currentScale;

        ChangeState(State.Idle);
    }

    private void Update()
    {
        UpdateState(CurrentState);

        HandleCurrentScale();
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

            // if not attacking, don't push other player
            if (CurrentState == State.Idle || Vector3.Dot(movementDirection, directionToOtherPlayer) < 0) return;

            otherPlayer.Push(directionToOtherPlayer, basePushForce + currentScale * pushForceLinearGrowth);
            otherPlayer.TakeDamage(pushDamagePercent);
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
        rigidBody.MovePosition(transform.position + Time.fixedDeltaTime * totalMoveSpeed * movementDirection);
    }

    private void HandleCurrentScale()
    {
        transform.localScale = currentScale * Vector3.one;

        rigidBody.mass = 4f/3f * Mathf.PI * Mathf.Pow(currentScale/2f, 3);
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

    public void TakeDamage(float damagePercent)
    {
        currentScale = Mathf.Max(startingScale, currentScale * (1f - damagePercent / 100f));
    }
}
