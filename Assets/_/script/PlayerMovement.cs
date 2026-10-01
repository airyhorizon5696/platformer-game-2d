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
    public float fallMultiplier = 2.5f; // Rend la chute plus rapide/snappy

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 2f;     // Temps minimum entre deux dashs

    [Header("Détection du sol")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
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

        // Le dash se recharge seulement au contact du sol
        if (isGrounded) dashReady = true;
        dashCooldownTimer -= Time.deltaTime;

        // Saut : hauteur fixe, un seul clic
        bool jumpPressed = kb[jumpKey1].wasPressedThisFrame || kb[jumpKey2].wasPressedThisFrame;
        if (jumpPressed && isGrounded && !isDashing)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (anim != null) anim.SetTrigger("Jump");
        }

        // Dash : nécessite le sol touché depuis le dernier dash ET le cooldown écoulé
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

        if (rb.linearVelocity.y < 0)
        {
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
    }
}
