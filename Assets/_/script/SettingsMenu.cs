using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private GameObject menuPanel;   // Le panneau du menu (désactivé au départ)

    [Header("Réglages")]
    [SerializeField] private string homeSceneName = "MainMenu"; // Nom exact de ta scène d'accueil
    [SerializeField] private bool pauseGameWhenOpen = true;
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape; // Touche pour ouvrir/fermer (optionnel)

    private bool isOpen;

    private void Start()
    {
        if (menuPanel != null) menuPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleMenu();
    }

    // À brancher sur le OnClick() du bouton engrenage
    public void ToggleMenu()
    {
        isOpen = !isOpen;
        menuPanel.SetActive(isOpen);

        if (pauseGameWhenOpen)
            Time.timeScale = isOpen ? 0f : 1f;
    }

    // À brancher sur le bouton "Retour à l'accueil"
    public void GoHome()
    {
        Time.timeScale = 1f; // Important : sinon la prochaine scène reste en pause
        SceneManager.LoadScene(homeSceneName);
    }

    // À brancher sur le bouton "Quitter"
    public void Quit()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
