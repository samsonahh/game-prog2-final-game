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
    private Color targetBodyColor;

    public void Init(Player newHolder)
    {
        holder = newHolder;

        bombTimer = 0f;

        holder.OnTouchOtherPlayer += Player_OnTouchOtherPlayer;

        tickRate = baseTickRate;
        tickTimer = 0f;

        DOTween.KillAll();

        DOTween.To(() => tickRate, x => tickRate = x, 0.1f, bombDuration).SetId("Tick");

        transform.localScale = Vector3.one;
        targetBodyColor = Color.black;
        SetColor(Color.black);
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        bodyMaterial = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        HandleBombTickingVisual();

        bombTimer += Time.deltaTime;

        if(bombTimer > bombDuration)
        {
            Explode();
            return;
        }
    }

    private void FixedUpdate()
    {
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

        transform.position = Vector3.Lerp(transform.position, holder.transform.position + 1.25f * Vector3.up, 100f * Time.fixedDeltaTime);
    }

    private void Explode()
    {
        CameraShaker.Instance.ShakeCamera(12f, 0.2f);

        Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);

        holder.OnTouchOtherPlayer -= Player_OnTouchOtherPlayer;

        gameManager.OnBombExplode(holder);

        DOTween.Kill("Tick");
        targetBodyColor = Color.black;
        SetColor(Color.black);

        holder.Kill();
        gameObject.SetActive(false);
    }

    private void HandleBombTickingVisual()
    {
        tickTimer += Time.deltaTime;

        if (tickTimer > tickRate)
        {
            tickTimer = 0f;

            DOTween.Kill("Color");
            DOTween.Kill(transform);

            DOVirtual.Color(targetBodyColor, targetBodyColor == Color.black ? Color.red : Color.black, tickRate, color => SetColor(color))
                .SetEase(Ease.OutQuint)
                .SetId("Color");
            targetBodyColor = targetBodyColor == Color.black ? Color.red : Color.black;
            
            transform.DOScale(transform.localScale == Vector3.one ? 1.1f * Vector3.one : Vector3.one, tickRate).SetEase(Ease.OutQuint);
        }
    }

    private void SetColor(Color color)
    {
        bodyMaterial.SetColor("_BaseColor", color);
    }
}
