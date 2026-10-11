using UnityEngine;
using UnityEngine.InputSystem;

// À mettre sur CHAQUE couche de fond du lobby (une même caméra fixe, pas besoin de la référencer).
// Plus "amplitude" est grande, plus la couche bouge = plus elle semble proche du joueur.
// Exemple : ciel lointain 0.1, montagnes 0.3, arbres 0.6, avant-plan 1.
public class MouseParallax : MonoBehaviour
{
    [Header("Effet")]
    public Vector2 amplitude = new Vector2(0.5f, 0.25f); // Déplacement max (en unités) à gauche/droite et haut/bas
    public float lissage = 6f;                            // Plus grand = suit la souris plus vite
    public bool inverser = true;                          // true = le fond bouge à l'opposé de la souris (effet "le joueur regarde")

    private Vector3 positionDepart;

    void Start()
    {
        positionDepart = transform.position;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Position de la souris ramenée entre -1 et 1 par rapport au centre de l'écran
        Vector2 souris = Mouse.current.position.ReadValue();
        float x = (souris.x / Screen.width) * 2f - 1f;
        float y = (souris.y / Screen.height) * 2f - 1f;
        Vector2 normalisee = new Vector2(Mathf.Clamp(x, -1f, 1f), Mathf.Clamp(y, -1f, 1f));

        float signe = inverser ? -1f : 1f;
        Vector3 cible = positionDepart + new Vector3(
            normalisee.x * amplitude.x * signe,
            normalisee.y * amplitude.y * signe,
            0f);

        // Déplacement doux vers la cible (pas de saccades), même si le jeu est en pause
        transform.position = Vector3.Lerp(transform.position, cible, lissage * Time.unscaledDeltaTime);
    }
}
