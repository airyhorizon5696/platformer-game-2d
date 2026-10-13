using UnityEngine;
using UnityEngine.InputSystem;

// À accrocher sur le même GameObject Joueur que PlayerMovement
// Gère 3 attaques différentes déclenchées par 3 touches distinctes
public class PlayerCombat : MonoBehaviour
{
    [Header("Touches d'attaque (modifiables ici)")]
    public Key attack1Key = Key.J;
    public Key attack2Key = Key.K;
    public Key attack3Key = Key.L;

    [Header("Réglages")]
    public float attackCooldown = 0.35f; // Temps minimum entre deux attaques (évite le spam)

    private Animator anim;
    private PlayerAudio sons;
    private float cooldownTimer = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sons = GetComponent<PlayerAudio>();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer > 0f) return; // On ignore les touches tant qu'on est en cooldown

        if (kb[attack1Key].wasPressedThisFrame)
        {
            TriggerAttack("Attack1", 1);
        }
        else if (kb[attack2Key].wasPressedThisFrame)
        {
            TriggerAttack("Attack2", 2);
        }
        else if (kb[attack3Key].wasPressedThisFrame)
        {
            TriggerAttack("Attack3", 3);
        }
    }

    void TriggerAttack(string triggerName, int numero)
    {
        cooldownTimer = attackCooldown;
        if (sons != null) sons.JouerAttaque(numero);
        if (anim != null)
        {
            anim.SetTrigger(triggerName);
        }


    }
}