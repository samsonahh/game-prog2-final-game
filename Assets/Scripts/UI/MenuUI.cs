using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    private Button playButton;

    private void Awake()
    {
        playButton = GetComponentInChildren<Button>();

        playButton.onClick.AddListener(PlayButton_OnClick);
    }

    private void PlayButton_OnClick()
    {
        SceneManager.LoadScene("Main");
    }

    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(PlayButton_OnClick);
    }
}
