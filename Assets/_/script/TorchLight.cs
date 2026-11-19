using UnityEngine;
using UnityEngine.Rendering.Universal; // Pour Light2D (URP 2D)

// À mettre sur un objet "TorchLight" ENFANT du joueur, qui a un composant Light 2D (type Point).
[RequireComponent(typeof(Light2D))]
public class TorchLight : MonoBehaviour
{
    [Header("Rayon de la lumière")]
    public float rayon = 6f;

    [Header("Effet de feu (vacillement)")]
    public float vitesseVacillement = 6f;
    public float amplitudeRayon = 0.25f;      // De combien le rayon pulse (en unités)
    public float amplitudeIntensite = 0.2f;   // De combien l'intensité varie
    public float intensiteDeBase = 1.2f;

    [Header("Couleur")]
    public Color couleurFeu = new Color(1f, 0.55f, 0.15f);

    private Light2D lumiere;
    private float decalage; // Évite que plusieurs torches vacillent exactement pareil

    void Awake()
    {
        lumiere = GetComponent<Light2D>();
        lumiere.lightType = Light2D.LightType.Point;
        lumiere.color = couleurFeu;
        decalage = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Vacillement : bruit de Perlin (plus naturel qu'un simple random)
        float t = Time.time * vitesseVacillement + decalage;
        float bruitRapide = Mathf.PerlinNoise(t, 0f);
        float bruitLent = Mathf.PerlinNoise(t * 0.3f, 10f);
        float vacillement = (bruitRapide * 0.7f + bruitLent * 0.3f) - 0.5f;

        lumiere.pointLightOuterRadius = rayon + vacillement * amplitudeRayon * 2f;
        lumiere.pointLightInnerRadius = lumiere.pointLightOuterRadius * 0.15f;
        lumiere.intensity = intensiteDeBase + vacillement * amplitudeIntensite * 2f;
    }
}