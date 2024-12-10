using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private GameManager gameManager;
    private Material bodyMaterial;

    private Player holder;

    [Header("References")]
    [SerializeField] private GameObject explosionVFXPrefab;
    [SerializeField] private GameObject poofVFXPrefab;

    [Header("Settings")]
    [SerializeField] private float bombDuration = 15f;
    private float bombTimer = 0f;
    [SerializeField] private float baseTickRate = 1f;
    private float tickRate;
    private float tickTimer = 0f;

    public void Init(Player newHolder)
    {
        holder = newHolder;

        bombTimer = 0f;

        holder.OnTouchOtherPlayer += Player_OnTouchOtherPlayer;

        tickRate = baseTickRate/2;
        tickTimer = 0f;

        DOTween.To(() => tickRate, x => tickRate = x, 0.05f, bombDuration).SetId("Tick");
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        bodyMaterial = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        tickTimer += Time.deltaTime;

        if(tickTimer > tickRate)
        {
            tickTimer = 0f;

            bodyMaterial.color = bodyMaterial.color == Color.black ? Color.red : Color.black;
        }

        bombTimer += Time.deltaTime;

        if(bombTimer > bombDuration)
        {
            Explode();
            return;
        }

        AttachToHolder();
    }

    private void Player_OnTouchOtherPlayer(Player otherPlayer, Vector3 contactPoint)
    {
        StartCoroutine(SwitchHolderCoroutine(otherPlayer, contactPoint));
    }

    private IEnumerator SwitchHolderCoroutine(Player newHolder, Vector3 contactPoint)
    {
        yield return null;

        CameraShaker.Instance.ShakeCamera(6f, 0.2f);

        Instantiate(poofVFXPrefab, contactPoint, Quaternion.identity);

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

        Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);

        holder.OnTouchOtherPlayer -= Player_OnTouchOtherPlayer;

        gameManager.OnBombExplode(holder);

        DOTween.Kill("Tick");
        bodyMaterial.color = Color.black;

        holder.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
