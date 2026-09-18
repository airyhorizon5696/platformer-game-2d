using UnityEngine;
using UnityEngine.InputSystem;

// À accrocher sur ton GameObject Joueur (celui qui a le Rigidbody2D et le Collider2D)
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Touches (modifiables ici)")]
    public Key moveLeftKey = Key.A;
    public Key moveRightKey = Key.D;
    public Key jumpKey1 = Key.W;
    public Key jumpKey2 = Key.Space;
    public Key dashKey = Key.LeftShift;

    [Header("Mouvement")]
    public float moveSpeed = 6f;

    [Header("Saut (hauteur fixe, un seul clic)")]
    public float jumpForce = 12f;
    public float fallMultiplier = 2.5f; // Rend la chute plus rapide/snappy (n'affecte pas la hauteur du saut)

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 2f;     // Temps minimum entre deux dashs, même si tu retouches le sol avant

    [Header("Détection du sol")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    [Header("Glissade au mur")]
    public Transform wallCheckRight;    // Objet vide placé sur le bord droit du collider
    public Transform wallCheckLeft;     // Objet vide placé sur le bord gauche du collider
    public float wallCheckRadius = 0.1f;
    public LayerMask wallLayer;         // Peut être la même Layer que Ground
    public float wallSlideSpeed = 1.5f; // Vitesse de descente lente le long du mur

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isTouchingWall;
    private float horizontalInput;
    private bool facingRight = true;

    private bool isDashing = false;
    private float dashTimer = 0f;
    private bool dashReady = true;      // Se remet à true seulement au contact du sol
    private float dashCooldownTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        // Mouvement horizontal
        horizontalInput = 0f;
        if (kb[moveLeftKey].isPressed) horizontalInput -= 1f;
        if (kb[moveRightKey].isPressed) horizontalInput += 1f;

        // Détection du sol
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Détection des murs (gauche ou droite)
        bool touchingRightWall = wallCheckRight != null && Physics2D.OverlapCircle(wallCheckRight.position, wallCheckRadius, wallLayer);
        bool touchingLeftWall = wallCheckLeft != null && Physics2D.OverlapCircle(wallCheckLeft.position, wallCheckRadius, wallLayer);
        isTouchingWall = touchingRightWall || touchingLeftWall;

        // Le dash se recharge seulement au contact du sol
        if (isGrounded) dashReady = true;
        dashCooldownTimer -= Time.deltaTime;

        // Saut : hauteur fixe, un seul clic, aucune variation possible en tenant la touche
        bool jumpPressed = kb[jumpKey1].wasPressedThisFrame || kb[jumpKey2].wasPressedThisFrame;
        if (jumpPressed && isGrounded && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (anim != null) anim.SetTrigger("Jump");
        }

        // Dash : nécessite d'avoir touché le sol depuis le dernier dash ET que le cooldown soit écoulé
        if (kb[dashKey].wasPressedThisFrame && dashReady && dashCooldownTimer <= 0f && !isDashing)
        {
            StartDash();
            if (anim != null) anim.SetTrigger("Dash");
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f) EndDash();
        }

        if (horizontalInput > 0 && !facingRight) Flip();
        else if (horizontalInput < 0 && facingRight) Flip();

        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(horizontalInput));
            anim.SetBool("IsGrounded", isGrounded);
            anim.SetBool("IsWallSliding", isTouchingWall && !isGrounded && rb.linearVelocity.y < 0f);
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            float dashDir = facingRight ? 1f : -1f;
            rb.linearVelocity = new Vector2(dashDir * dashSpeed, 0f);
            return;
        }

        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);

        // Glissade au mur : si on touche un mur, qu'on n'est pas au sol, et qu'on tombe -> ralentit la chute
        bool slidingOnWall = isTouchingWall && !isGrounded && rb.linearVelocity.y < 0f;
        if (slidingOnWall)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else if (rb.linearVelocity.y < 0)
        {
            // Chute normale (plus rapide/snappy), pas de logique liée à la durée d'appui du saut
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashReady = false;
        dashCooldownTimer = dashCooldown;
        dashTimer = dashDuration;
        rb.gravityScale = 0f;
    }

    void EndDash()
    {
        isDashing = false;
        rb.gravityScale = 1f;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null) Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        if (wallCheckRight != null) Gizmos.DrawWireSphere(wallCheckRight.position, wallCheckRadius);
        if (wallCheckLeft != null) Gizmos.DrawWireSphere(wallCheckLeft.position, wallCheckRadius);
    }
}
