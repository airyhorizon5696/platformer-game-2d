using UnityEngine;
using UnityEngine.SceneManagement;

// Met ce script sur UN SEUL objet vide dans ta toute première scène (ex: "GameManager").
// Grâce à DontDestroyOnLoad, cet objet survit à tous les changements de scène au lieu
// d'être détruit et recréé à chaque fois — c'est ce qui permet de garder des infos en mémoire.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Accessible depuis n'importe quel script via GameManager.Instance

    [Header("Progression du joueur (persiste entre les scènes)")]
    public bool doubleSautDebloque = false;
    public bool torcheTrouvee = false; // Passe à true quand le joueur ramasse la torche au niveau 1

    // À appeler depuis un Invoke Event (CES) sur l'objet torche du niveau 1
    public void TrouverTorche()
    {
        torcheTrouvee = true;
    }

    [Header("Point de réapparition actuel")]
    public string sceneDeReapparition;      // Nom de la scène où le joueur doit réapparaître
    public Vector3 positionDeReapparition;  // Position dans cette scène

    void Awake()
    {
        // S'il existe déjà un GameManager (ex: on revient sur la scène 1 qui en a un autre), on se détruit
        // pour n'en garder qu'un seul en permanence.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // À appeler depuis un Invoke Event (CES) sur un point de checkpoint (ex: l'entrée de la grotte)
    public void DefinirCheckpoint(Transform point)
    {
        sceneDeReapparition = SceneManager.GetActiveScene().name;
        positionDeReapparition = point.position;
    }

    // À appeler depuis un Invoke Event (CES) pour changer de scène normalement (ex: entrer dans la grotte)
    public void ChangerDeScene(string nomDeScene)
    {
        SceneManager.LoadScene(nomDeScene);
    }

    // À appeler quand le joueur meurt / tombe dans le vide : recharge la scène du dernier checkpoint
    public void RespawnAuDernierCheckpoint()
    {
        if (string.IsNullOrEmpty(sceneDeReapparition))
        {
            // Aucun checkpoint enregistré encore : recharge juste la scène actuelle
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene(sceneDeReapparition);
        }
    }
}
