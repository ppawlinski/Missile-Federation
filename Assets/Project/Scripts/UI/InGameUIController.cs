using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject matchManagerObject;
    [SerializeField] GameObject scoreBoard;
    bool pauseMenuOpen = false;

    public delegate void PauseHandler(bool value);
    public static event PauseHandler OnGamePause;

    void OnEscapeMenuToggle()
    {
        pauseMenuOpen = !pauseMenuOpen;
        Time.timeScale = pauseMenuOpen ? 0 : 1;
        pauseMenu.SetActive(pauseMenuOpen);
        OnGamePause?.Invoke(pauseMenuOpen);
    }

    void OnScoreboardToggle(InputValue value)
    {
        scoreBoard.GetComponent<Canvas>().enabled = Convert.ToBoolean(value.Get<float>());
    }
}
