using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;     // fond sombre + tout le reste
    public GameObject menuPanel;      // Resume / Controls / Quit
    public GameObject controlsPanel;  // écran des contrôles
    public string mainMenuScene = ""; // laisse vide pour fermer le jeu
    public bool lockCursorInGame = true; // décoche si ton jeu utilise un curseur visible

    bool paused;

    void Start()
    {
        Resume();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!paused)
            {
                Pause();
            }
            else if (controlsPanel.activeSelf)
            {
                HideControls();
            }
            else
            {
                Resume();
            }
        }
    }

    void EnableAllButtons()
    {
        foreach (Transform child in menuPanel.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void Pause()
    {
        paused = true;
        pausePanel.SetActive(true);
        menuPanel.SetActive(true);
        EnableAllButtons();
        controlsPanel.SetActive(false);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        paused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        if (lockCursorInGame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ShowControls()
    {
        menuPanel.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        controlsPanel.SetActive(false);
        menuPanel.SetActive(true);
        EnableAllButtons();
    }

    public void Quit()
    {
        Time.timeScale = 1f;
        if (string.IsNullOrEmpty("Accueil"))
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene("Accueil");
        }
    }
}