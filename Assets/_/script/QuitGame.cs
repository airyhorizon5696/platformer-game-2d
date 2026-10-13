using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // À brancher sur le OnClick() du bouton "Quitter"
    public void Quit()
    {
#if UNITY_EDITOR
        // Dans l'éditeur, Application.Quit() ne fait rien : on arrête le mode Play
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
