using UnityEngine;
using UnityEngine.Audio;

// À mettre sur ton Joueur (avec PlayerMovement et PlayerCombat).
// Pour chaque action, glisse 2-4 petits sons différents dans la liste : le script en choisit un au hasard
// (jamais le même deux fois de suite) et varie légèrement la hauteur. C'est LE truc qui évite
// que les sons deviennent agaçants quand ils se répètent.
public class PlayerAudio : MonoBehaviour
{
    [System.Serializable]
    public class GroupeSons
    {
        public AudioClip[] clips;
        [Range(0f, 1f)] public float volume = 0.5f;
        [Range(0f, 0.4f)] public float variationHauteur = 0.08f; // 0.08 = ±8% de variation de pitch

        [System.NonSerialized] public int dernierIndex = -1;
    }

    [Header("Sons du joueur")]
    public GroupeSons saut;
    public GroupeSons doubleSaut;
    public GroupeSons atterrissage;
    public GroupeSons dash;
    public GroupeSons attaque1;
    public GroupeSons attaque2;
    public GroupeSons attaque3;
    public GroupeSons degats;
    public GroupeSons mort;
    public GroupeSons ramassage;

    [Header("Sortie (optionnel)")]
    [Tooltip("Si tu crées un AudioMixer avec un groupe 'SFX', glisse-le ici pour régler le volume des effets séparément de la musique")]
    public AudioMixerGroup groupeMixer;

    private AudioSource[] sources;
    private int sourceSuivante = 0;

    void Awake()
    {
        // Petit pool de 6 AudioSources : permet de jouer plusieurs sons en même temps
        // avec des hauteurs différentes sans qu'ils se coupent entre eux.
        sources = new AudioSource[6];
        for (int i = 0; i < sources.Length; i++)
        {
            sources[i] = gameObject.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
            sources[i].spatialBlend = 0f; // 0 = son 2D (même volume partout, ce qu'on veut en platformer 2D)
            sources[i].outputAudioMixerGroup = groupeMixer;
        }
    }

    // --- Méthodes appelées par PlayerMovement / PlayerCombat ---
    public void JouerSaut() { Jouer(saut); }
    public void JouerDoubleSaut() { Jouer(doubleSaut); }
    public void JouerAtterrissage() { Jouer(atterrissage); }
    public void JouerDash() { Jouer(dash); }
    public void JouerDegats() { Jouer(degats); }
    public void JouerMort() { Jouer(mort); }
    public void JouerRamassage() { Jouer(ramassage); }

    public void JouerAttaque(int numero)
    {
        switch (numero)
        {
            case 1: Jouer(attaque1); break;
            case 2: Jouer(attaque2); break;
            case 3: Jouer(attaque3); break;
        }
    }

    // Utilisable depuis un Invoke Event (CES) pour jouer un son précis (ex: ouverture d'une porte)
    public void JouerSon(AudioClip clip)
    {
        if (clip == null) return;
        JouerClip(clip, 0.6f, 0f);
    }

    // --- Interne ---
    void Jouer(GroupeSons groupe)
    {
        if (groupe == null || groupe.clips == null || groupe.clips.Length == 0) return;

        // Choisit un son au hasard, mais jamais le même que la fois d'avant (si on en a plusieurs)
        int index = Random.Range(0, groupe.clips.Length);
        if (groupe.clips.Length > 1 && index == groupe.dernierIndex)
        {
            index = (index + 1) % groupe.clips.Length;
        }
        groupe.dernierIndex = index;

        JouerClip(groupe.clips[index], groupe.volume, groupe.variationHauteur);
    }

    void JouerClip(AudioClip clip, float volume, float variationHauteur)
    {
        if (clip == null) return;

        AudioSource source = sources[sourceSuivante];
        sourceSuivante = (sourceSuivante + 1) % sources.Length;

        source.pitch = 1f + Random.Range(-variationHauteur, variationHauteur);
        source.PlayOneShot(clip, volume);
    }
}
