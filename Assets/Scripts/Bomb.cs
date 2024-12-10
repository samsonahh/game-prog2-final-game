    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private GameManager gameManager;

    private Player holder;

    [Header("Settings")]
    [SerializeField] private float bombDuration = 15f;
    private float bombTimer = 0f;

    public void Init(Player newHolder)
    {
        holder = newHolder;

        bombTimer = 0f;

        holder.OnTouchOtherPlayer += Player_OnTouchOtherPlayer;
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        bombTimer += Time.deltaTime;

        if(bombTimer > bombDuration)
        {
            Explode();
            return;
        }

        AttachToHolder();
    }

    private void Player_OnTouchOtherPlayer(Player otherPlayer)
    {
        StartCoroutine(SwitchHolderCoroutine(otherPlayer));
    }

    private IEnumerator SwitchHolderCoroutine(Player newHolder)
    {
        yield return null;

        CameraShaker.Instance.ShakeCamera(6f, 0.2f);

        holder.OnTouchOtherPlayer -= Player_OnTouchOtherPlayer;

        holder = newHolder;

        holder.OnTouchOtherPlayer += Player_OnTouchOtherPlayer;
    }

    private void AttachToHolder()
    {
        if (holder == null) return;

        transform.position = holder.transform.position + Vector3.up;
    }

    private void Explode()
    {
        CameraShaker.Instance.ShakeCamera(12f, 0.2f);

        holder.OnTouchOtherPlayer -= Player_OnTouchOtherPlayer;

        gameManager.OnBombExplode(holder);

        holder.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
