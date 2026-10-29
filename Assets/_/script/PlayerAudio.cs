using UnityEngine;

// À mettre sur ton Joueur (avec PlayerMovement et PlayerCombat).
// - Pour chaque action, glisse 2-4 petits sons : le script en choisit un au hasard (jamais le même
//   deux fois de suite) avec une légère variation de hauteur ET de volume.
// - Les PAS jouent quand tu marches sur le sol (et peuvent changer selon le Tag du sol, si tu veux).
// - Le MIXAGE (volumes) est directement ici, section "Mixage", pas besoin d'AudioMixer.
// - Section "Douceur" : fondus d'entrée/sortie sur chaque son (plus de clics secs), changements de
//   volume progressifs, et les pas baissent automatiquement quand une action plus importante joue.
public class PlayerAudio : MonoBehaviour
{
    [System.Serializable]
    public class GroupeSons
    {
        public AudioClip[] clips;
        [Range(0f, 1f)] public float volume = 0.5f;
        [Range(0f, 0.4f)] public float variationHauteur = 0.08f;  // 0.08 = ±8% de variation de pitch
        [Range(0f, 0.5f)] public float variationVolume = 0.1f;    // 0.1 = ±10% de variation de volume

        [System.NonSerialized] public int dernierIndex = -1;
    }

    [System.Serializable]
    public class Surface
    {
        public string nom = "Terre"; // Juste pour t'y retrouver dans l'Inspector
        [Tooltip("Le Tag Unity des objets de ce sol. Doit être écrit EXACTEMENT pareil (majuscules incluses).")]
        public string tagDuSol = "Terre";
        public GroupeSons pas = new GroupeSons();
        [Tooltip("Optionnel : son d'atterrissage propre à ce sol. Si vide, on utilise celui du groupe 'Atterrissage' plus bas.")]
        public GroupeSons atterrissage = new GroupeSons();
    }

    enum Categorie { Mouvement, Pas, Combat, Autres }

    // Une "voix" = un son en train de jouer, avec son enveloppe (fondu d'entrée/sortie)
    class Voix
    {
        public AudioSource source;
        public Categorie categorie;
        public float volumeBase;
        public float temps;
        public float duree;
        public bool enCours;
    }

    // =====================================================================
    //  MIXAGE : tous les volumes sont ici. Chaque son est multiplié par le
    //  volume de sa catégorie, puis par le volume général.
    // =====================================================================
    [Header("Mixage (tous les volumes sont ici)")]
    [Range(0f, 1f)] public float volumeGeneral = 1f;
    [Range(0f, 1f)] public float volumeMouvements = 1f; // saut, double saut, atterrissage, dash
    [Range(0f, 1f)] public float volumePas = 1f;
    [Range(0f, 1f)] public float volumeCombat = 1f;     // attaques
    [Range(0f, 1f)] public float volumeAutres = 1f;     // dégâts, mort, ramassage, JouerSon
    public bool muet = false;

    // =====================================================================
    //  DOUCEUR : ce qui rend le tout moins brusque
    // =====================================================================
    [Header("Douceur (anti-brusque)")]
    [Tooltip("Montée du volume au début de chaque son (en secondes). Supprime les 'clics' secs. 0 = désactivé.")]
    [Range(0f, 0.05f)] public float fonduEntree = 0.01f;
    [Tooltip("Descente du volume à la fin de chaque son (en secondes). 0 = désactivé.")]
    [Range(0f, 0.3f)] public float fonduSortie = 0.05f;
    [Tooltip("Vitesse des changements de volume général / muet (plus petit = plus lent et doux).")]
    public float vitesseChangementVolume = 4f;
    [Tooltip("Les pas baissent doucement quand un son plus important joue (saut, dash, attaque...), pour éviter l'empilement.")]
    public bool reduirePasQuandActions = true;
    [Range(0f, 1f)] public float facteurReductionPas = 0.4f; // 0.4 = les pas passent à 40% de leur volume
    public float dureeReduction = 0.25f;

    [Header("Sons de mouvement")]
    public GroupeSons saut;
    public GroupeSons doubleSaut;
    public GroupeSons atterrissage;
    public GroupeSons dash;

    [Header("Sons de combat")]
    public GroupeSons attaque1;
    public GroupeSons attaque2;
    public GroupeSons attaque3;

    [Header("Autres sons")]
    public GroupeSons degats;
    public GroupeSons mort;
    public GroupeSons ramassage;

    [Header("Pas selon le sol")]
    [Tooltip("Coché : les pas se jouent tout seuls pendant que tu marches. Décoché : appelle JouerPas() depuis des Animation Events.")]
    public bool pasAutomatiques = true;
    public float intervallePas = 0.35f;   // Temps entre deux pas à pleine vitesse (plus petit = pas plus rapides)
    [Range(0f, 0.5f)] public float variationIntervallePas = 0.12f; // Rythme légèrement irrégulier = plus naturel
    public float seuilVitesse = 0.5f;     // Vitesse horizontale minimum pour compter comme "en marche"
    [Tooltip("Sons utilisés si le sol n'a aucun Tag reconnu dans la liste ci-dessous.")]
    public GroupeSons pasParDefaut;
    public Surface[] surfaces;

    private const int NOMBRE_DE_VOIX = 10;

    private Voix[] voix;
    private PlayerMovement mouvement;
    private Rigidbody2D rb;
    private float minuteurPas = 0f;

    private float generalLisse;       // volume général qui suit volumeGeneral en douceur
    private float reductionActuelle = 1f;
    private float minuteurReduction = 0f;

    void Awake()
    {
        mouvement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        generalLisse = muet ? 0f : volumeGeneral;

        // Pool de voix : permet de jouer plusieurs sons en même temps (avec des hauteurs différentes)
        // sans qu'ils se coupent entre eux.
        voix = new Voix[NOMBRE_DE_VOIX];
        for (int i = 0; i < voix.Length; i++)
        {
            Voix v = new Voix();
            v.source = gameObject.AddComponent<AudioSource>();
            v.source.playOnAwake = false;
            v.source.loop = false;
            v.source.spatialBlend = 0f; // 0 = son 2D (même volume partout)
            voix[i] = v;
        }
    }

    void Update()
    {
        MettreAJourMixage();
        MettreAJourPas();
    }

    // Applique les fondus et les volumes en continu sur tous les sons en cours
    void MettreAJourMixage()
    {
        float dt = Time.unscaledDeltaTime;

        // Volume général : suit sa cible en douceur (un "muet" devient un fondu, pas une coupure sèche)
        float cibleGenerale = muet ? 0f : volumeGeneral;
        generalLisse = Mathf.MoveTowards(generalLisse, cibleGenerale, vitesseChangementVolume * dt);

        // Réduction des pas : descend vite, remonte lentement (plus naturel)
        minuteurReduction -= dt;
        float cibleReduction = (reduirePasQuandActions && minuteurReduction > 0f) ? facteurReductionPas : 1f;
        float vitesseReduction = (cibleReduction < reductionActuelle) ? 10f : 2.5f;
        reductionActuelle = Mathf.MoveTowards(reductionActuelle, cibleReduction, vitesseReduction * dt);

        foreach (Voix v in voix)
        {
            if (!v.enCours) continue;

            v.temps += dt;
            if (v.temps >= v.duree || (v.temps > 0.05f && !v.source.isPlaying))
            {
                v.enCours = false;
                continue;
            }

            // Enveloppe : montée au début, descente à la fin (limitée pour les sons très courts)
            float entree = Mathf.Min(fonduEntree, v.duree * 0.3f);
            float sortie = Mathf.Min(fonduSortie, v.duree * 0.4f);

            float enveloppe = 1f;
            if (entree > 0f) enveloppe = Mathf.Min(enveloppe, v.temps / entree);
            if (sortie > 0f) enveloppe = Mathf.Min(enveloppe, (v.duree - v.temps) / sortie);
            enveloppe = Mathf.Clamp01(enveloppe);

            float volume = v.volumeBase * VolumeCategorie(v.categorie) * generalLisse * enveloppe;
            if (v.categorie == Categorie.Pas) volume *= reductionActuelle;

            v.source.volume = Mathf.Clamp01(volume);
        }
    }

    void MettreAJourPas()
    {
        if (!pasAutomatiques || mouvement == null || rb == null) return;

        float vitesse = Mathf.Abs(rb.linearVelocity.x);
        bool enMarche = mouvement.IsGrounded && !mouvement.IsDashing && vitesse > seuilVitesse;

        if (!enMarche)
        {
            minuteurPas = 0f; // Le prochain pas jouera tout de suite quand on repart
            return;
        }

        minuteurPas -= Time.deltaTime;
        if (minuteurPas <= 0f)
        {
            JouerPas();

            // Rythme : un peu plus lent si on va moins vite, avec une petite irrégularité naturelle
            float facteurVitesse = 1f;
            if (mouvement.moveSpeed > 0.01f)
                facteurVitesse = Mathf.Clamp(mouvement.moveSpeed / vitesse, 0.7f, 1.4f);

            minuteurPas = intervallePas * facteurVitesse
                          * (1f + Random.Range(-variationIntervallePas, variationIntervallePas));
        }
    }

    // --- Méthodes appelées par PlayerMovement / PlayerCombat ---
    public void JouerSaut() { Jouer(saut, Categorie.Mouvement); }
    public void JouerDoubleSaut() { Jouer(doubleSaut, Categorie.Mouvement); }
    public void JouerDash() { Jouer(dash, Categorie.Mouvement); }
    public void JouerDegats() { Jouer(degats, Categorie.Autres); }
    public void JouerMort() { Jouer(mort, Categorie.Autres); }
    public void JouerRamassage() { Jouer(ramassage, Categorie.Autres); }

    public void JouerAtterrissage()
    {
        Surface s = TrouverSurface();
        if (s != null && ADesClips(s.atterrissage)) Jouer(s.atterrissage, Categorie.Mouvement);
        else Jouer(atterrissage, Categorie.Mouvement);

        // Évite qu'un pas parte pile en même temps que l'atterrissage
        minuteurPas = Mathf.Max(minuteurPas, intervallePas * 0.6f);
    }

    // Public : tu peux aussi l'appeler depuis un Animation Event (sur les frames où le pied touche le sol)
    public void JouerPas()
    {
        Surface s = TrouverSurface();
        if (s != null && ADesClips(s.pas)) Jouer(s.pas, Categorie.Pas);
        else Jouer(pasParDefaut, Categorie.Pas);
    }

    public void JouerAttaque(int numero)
    {
        switch (numero)
        {
            case 1: Jouer(attaque1, Categorie.Combat); break;
            case 2: Jouer(attaque2, Categorie.Combat); break;
            case 3: Jouer(attaque3, Categorie.Combat); break;
        }
    }

    // Utilisable depuis un Invoke Event (CES) pour jouer un son précis (ex: ouverture d'une porte)
    public void JouerSon(AudioClip clip)
    {
        if (muet || clip == null) return;
        JouerClip(clip, 0.6f, 0f, 0f, Categorie.Autres);
    }

    // Utiles pour un futur menu d'options (le changement se fait en douceur)
    public void DefinirVolumeGeneral(float valeur) { volumeGeneral = Mathf.Clamp01(valeur); }
    public void DefinirMuet(bool valeur) { muet = valeur; }

    // --- Interne ---
    // Regarde sous les pieds du joueur et renvoie la Surface dont le Tag correspond (ou null)
    Surface TrouverSurface()
    {
        if (mouvement == null || mouvement.groundCheck == null || surfaces == null) return null;

        Collider2D sol = Physics2D.OverlapCircle(
            mouvement.groundCheck.position,
            mouvement.groundCheckRadius + 0.1f,
            mouvement.groundLayer);
        if (sol == null) return null;

        string tagSol = sol.gameObject.tag;
        foreach (Surface s in surfaces)
        {
            if (s != null && s.tagDuSol == tagSol) return s;
        }
        return null;
    }

    bool ADesClips(GroupeSons groupe)
    {
        return groupe != null && groupe.clips != null && groupe.clips.Length > 0;
    }

    float VolumeCategorie(Categorie categorie)
    {
        switch (categorie)
        {
            case Categorie.Mouvement: return volumeMouvements;
            case Categorie.Pas: return volumePas;
            case Categorie.Combat: return volumeCombat;
            default: return volumeAutres;
        }
    }

    void Jouer(GroupeSons groupe, Categorie categorie)
    {
        if (muet || !ADesClips(groupe)) return;

        // Choisit un son au hasard, mais jamais le même que la fois d'avant (si on en a plusieurs)
        int index = Random.Range(0, groupe.clips.Length);
        if (groupe.clips.Length > 1 && index == groupe.dernierIndex)
        {
            index = (index + 1) % groupe.clips.Length;
        }
        groupe.dernierIndex = index;

        JouerClip(groupe.clips[index], groupe.volume, groupe.variationHauteur, groupe.variationVolume, categorie);
    }

    void JouerClip(AudioClip clip, float volumeBase, float variationHauteur, float variationVolume, Categorie categorie)
    {
        if (clip == null) return;

        Voix v = ObtenirVoix();

        float hauteur = 1f + Random.Range(-variationHauteur, variationHauteur);
        float facteurVolume = 1f + Random.Range(-variationVolume, variationVolume);

        v.source.clip = clip;
        v.source.pitch = hauteur;
        v.source.volume = 0f; // Démarre à 0 : c'est MettreAJourMixage qui fait monter le volume (fondu d'entrée)
        v.categorie = categorie;
        v.volumeBase = volumeBase * facteurVolume;
        v.temps = 0f;
        v.duree = clip.length / Mathf.Max(0.1f, Mathf.Abs(hauteur));
        v.enCours = true;
        v.source.Play();

        // Un son "important" fait baisser doucement les pas pendant un instant
        if (categorie != Categorie.Pas) minuteurReduction = dureeReduction;
    }

    Voix ObtenirVoix()
    {
        foreach (Voix v in voix)
        {
            if (!v.enCours) return v;
        }

        // Plus aucune voix libre (rare) : on réutilise la plus ancienne
        Voix plusAncienne = voix[0];
        foreach (Voix v in voix)
        {
            if (v.temps > plusAncienne.temps) plusAncienne = v;
        }
        plusAncienne.source.Stop();
        return plusAncienne;
    }
}