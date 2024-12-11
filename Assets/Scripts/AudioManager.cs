using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

    [field: Header("Clips")]
    [field: SerializeField] public AudioClip ExplosionSFX { get; private set; }
    [field: SerializeField] public AudioClip ContactSFX { get; private set; }
    [field: SerializeField] public AudioClip BeepSFX { get; private set; }
    [field: SerializeField] public AudioClip TickSFX { get; private set; }
    [field: SerializeField] public AudioClip StartSFX { get; private set; }
    [field: SerializeField] public AudioClip GameOverSFX { get; private set; }

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

        audioSource = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip, Vector3 position, float volume, float pitch = 1f)
    {
        GameObject oneShotAudioObject = new GameObject("One Shot Audio");
        oneShotAudioObject.transform.position = position;
        AudioSource audioSource = oneShotAudioObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.Play();

        Destroy(oneShotAudioObject, clip == null ? 0 : clip.length);
    }
}
