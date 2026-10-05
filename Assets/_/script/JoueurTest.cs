using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class JoueurTest : MonoBehaviour
{
    public float vitesse = 5f;
    public float forceSaut = 7f;

    Rigidbody2D rb;
    InputAction actionDeplacement;
    InputAction actionSaut;
    int sautsRestants;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        actionDeplacement = InputSystem.actions.FindAction("Move");
        actionSaut = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        float x = actionDeplacement.ReadValue<Vector2>().x;
        rb.linearVelocity = new Vector2(x * vitesse, rb.linearVelocity.y);

        if (actionSaut.WasPressedThisFrame() && sautsRestants > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, forceSaut);
            sautsRestants--;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // Touche le sol (pas un mur) : 1 saut, ou 2 si le double saut est débloqué
        if (col.GetContact(0).normal.y > 0.5f)
        {
            sautsRestants = DonneesJeu.doubleSautDebloque ? 2 : 1;
        }
    }
}