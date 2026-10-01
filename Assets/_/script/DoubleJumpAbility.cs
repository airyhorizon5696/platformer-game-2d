using UnityEngine;
using UnityEngine.InputSystem;
// Appelle DebloquerDoubleSaut() depuis un Invoke Event du Collider Event System
[RequireComponent(typeof(PlayerMovement))]
public class DoubleJumpAbility : MonoBehaviour
{
    [Header("Réglages du double saut")]
    public float extraJumpForce = 6f; 

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
            playerMovement.PerformJump(extraJumpForce);
            extraJumpUtilise = true;
        }
    }

    // Méthode publique à appeler depuis un Invoke Event (CES) quand le joueur ramasse l'objet
    public void DebloquerDoubleSaut()
    {
        debloque = true;
    }
}
