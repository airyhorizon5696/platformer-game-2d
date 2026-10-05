using UnityEngine;
using UnityEngine.SceneManagement;

public class Porte : MonoBehaviour
{
    public string idPorte;            // Identifiant unique de cette porte
    public string sceneCible;         // Scène à charger (string exacte)
    public string porteCible;         // Porte où apparaître dans la scène cible
    public Transform pointApparition; // Enfant vide placé à côté de la porte, hors du BoxCollider2D

    void Start()
    {
        if (DonneesJeu.porteDArrivee == idPorte)
        {
            // Ici c'est important de savoir que ton personnage doit avoir le tag "Player", sinon ça fonctionnera pas
            GameObject.FindWithTag("Player").transform.position = pointApparition.position;
            DonneesJeu.porteDArrivee = "";
        }
    }
    // Équivalent du OnTriggerEnter, mais en 2d
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DonneesJeu.porteDArrivee = porteCible;
            SceneManager.LoadScene(sceneCible);
        }
    }
}