using UnityEngine;
using UnityEngine.Rendering.Universal; // Pour Light2D (URP 2D)

// À mettre sur un objet "TorchLight" ENFANT du joueur, qui a un composant Light 2D (type Point).
// Garde le rayon petit tant que la torche n'est pas trouvée, puis l'agrandit en douceur.
[RequireComponent(typeof(Light2D))]
public class TorchLight : MonoBehaviour
{
    [Header("Rayon de la lumière")]
    public float rayonSansTorche = 2.5f;
    public float rayonAvecTorche = 6f;
    public float vitesseTransition = 1.5f;   // Vitesse à laquelle le rayon grandit quand on trouve la torche

    [Header("Effet de feu (vacillement)")]
    public float vitesseVacillement = 6f;
    public float amplitudeRayon = 0.25f;      // De combien le rayon pulse (en unités)
    public float amplitudeIntensite = 0.2f;   // De combien l'intensité varie
    public float intensiteDeBase = 1.2f;

    [Header("Couleurs (centre chaud -> bords plus rouges)")]
    public Color couleurFeu = new Color(1f, 0.55f, 0.15f);

    [Header("Test sans GameManager")]
    public bool torcheTrouveeDebug = false;   // Coche pour tester dans une scène lancée seule

    private Light2D lumiere;
    private float rayonActuel;
    private float decalage; // Évite que plusieurs torches vacillent exactement pareil

    void Awake()
    {
        lumiere = GetComponent<Light2D>();
        lumiere.lightType = Light2D.LightType.Point;
        lumiere.color = couleurFeu;
        decalage = Random.Range(0f, 100f);
        rayonActuel = ObtenirRayonCible();
    }

    void Update()
    {
        // Rayon cible selon que la torche est trouvée ou non, avec transition douce
        float rayonCible = ObtenirRayonCible();
        rayonActuel = Mathf.Lerp(rayonActuel, rayonCible, vitesseTransition * Time.deltaTime);

        // Vacillement : bruit de Perlin (plus naturel qu'un simple random)
        float t = Time.time * vitesseVacillement + decalage;
        float bruitRapide = Mathf.PerlinNoise(t, 0f);          // 0..1
        float bruitLent = Mathf.PerlinNoise(t * 0.3f, 10f);    // variation plus lente par-dessus

        float vacillement = (bruitRapide * 0.7f + bruitLent * 0.3f) - 0.5f; // environ -0.5..0.5

        lumiere.pointLightOuterRadius = rayonActuel + vacillement * amplitudeRayon * 2f;
        lumiere.pointLightInnerRadius = lumiere.pointLightOuterRadius * 0.15f;
        lumiere.intensity = intensiteDeBase + vacillement * amplitudeIntensite * 2f;
    }

    float ObtenirRayonCible()
    {
        bool torche = torcheTrouveeDebug;
        if (GameManager.Instance != null) torche = GameManager.Instance.torcheTrouvee;
        return torche ? rayonAvecTorche : rayonSansTorche;
    }
}
