using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }

    private Vector3 startPosition;

    private bool isCamShaking;
    private float camShakeTimer = Mathf.Infinity;
    private float camShakeMagnitude;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        UpdateCameraShake();
    }

    private void UpdateCameraShake()
    {
        if (!isCamShaking)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, 15f * Time.unscaledDeltaTime);
            return;
        }

        Vector3 randomShakeOffset = camShakeMagnitude * (Vector3)Random.insideUnitCircle.normalized;
        Vector3 targetShakePosition = startPosition + randomShakeOffset;

        camShakeTimer -= Time.unscaledDeltaTime;
        if (camShakeTimer <= 0) isCamShaking = false;

        transform.position = Vector3.Lerp(transform.position, targetShakePosition, Time.unscaledDeltaTime);
    }

    public void ShakeCamera(float magnitude, float duration)
    {
        isCamShaking = true;

        camShakeTimer = duration;
        camShakeMagnitude = magnitude;
    }
}
