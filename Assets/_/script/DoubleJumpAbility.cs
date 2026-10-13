using UnityEngine;
using UnityEngine.InputSystem;

// À ajouter sur le même GameObject que PlayerMovement.
// Le double saut est VERROUILLÉ par défaut (débloqué = false).

// (sur le trigger de ramassage de l'objet) pour l'activer.
[RequireComponent(typeof(PlayerMovement))]
public class DoubleJumpAbility : MonoBehaviour
{
    [Header("Réglages du double saut")]
    public float extraJumpForce = 9f; // Un peu plus faible que le saut normal, c'est souvent plus satisfaisant

    [Header("État (lecture seule, juste pour debug)")]
    public bool debloque = false;

    private PlayerMovement playerMovement;
    private bool extraJumpUtilise = false;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!debloque) return; // Capacité pas encore obtenue, on ne fait rien

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // Recharge le double saut dès qu'on retouche le sol
        if (playerMovement.IsGrounded)
        {
            extraJumpUtilise = false;
            return;
        }

        bool jumpPressed = kb[playerMovement.jumpKey1].wasPressedThisFrame || kb[playerMovement.jumpKey2].wasPressedThisFrame;

        if (jumpPressed && !extraJumpUtilise && !playerMovement.IsDashing)
        {
            playerMovement.PerformJump(extraJumpForce, true); // true = saut aérien (son différent)
            extraJumpUtilise = true;
        }
    }

    // Méthodes publiques à appeler depuis un Invoke Event (CES).
    // "debloque" est une variable publique : tu peux aussi la lire/l'afficher ailleurs si besoin,

    public void ActiverDoubleSaut()
    {
        debloque = true;
        if (GameManager.Instance != null) GameManager.Instance.doubleSautDebloque = true;
    }

    public void DesactiverDoubleSaut()
    {
        debloque = false;
        if (GameManager.Instance != null) GameManager.Instance.doubleSautDebloque = false;
    }
}