using UnityEngine;

// À accrocher sur la Caméra principale (pas sur le joueur)
public class CameraFollow : MonoBehaviour
{
    [Header("Cible")]
    public Transform target;           // Glisse ton Joueur ici dans l'Inspector

    [Header("Fluidité")]
    public float smoothTime = 0.2f;    // Plus petit = colle plus vite au joueur, plus grand = plus "mou"

    [Header("Décalage dynamique (façon Celeste)")]
    public float lookAheadDistance = 2f;   // Distance de décalage horizontal max
    public float lookAheadSpeed = 3f;      // Vitesse à laquelle le décalage se met à jour

    [Header("Limites (optionnel)")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector3 velocity = Vector3.zero;
    private float currentLookAhead = 0f;
    private float targetLookAhead = 0f;
    private Rigidbody2D targetRb;

    void Start()
    {
        if (target != null)
            targetRb = target.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calcule le décalage selon la direction où le joueur va (regarde "un peu plus loin" devant lui)
        if (targetRb != null && Mathf.Abs(targetRb.linearVelocity.x) > 0.1f)
        {
            targetLookAhead = Mathf.Sign(targetRb.linearVelocity.x) * lookAheadDistance;
        }
        currentLookAhead = Mathf.Lerp(currentLookAhead, targetLookAhead, lookAheadSpeed * Time.deltaTime);

        Vector3 desiredPosition = target.position + new Vector3(currentLookAhead, 0f, 0f);
        desiredPosition.z = transform.position.z; // Garde la distance Z de la caméra intacte

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        }

        // SmoothDamp = mouvement fluide, sans à-coups, contrairement à un simple Lerp
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
    }
}
