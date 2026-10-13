using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [Header("Sons (laisse vide pour désactiver)")]
    [SerializeField] private AudioClip hoverSound;   // souris qui passe dessus
    [SerializeField] private AudioClip selectSound;  // sélection clavier/manette
    [SerializeField] private AudioClip clickSound;   // clic

    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private AudioSource source;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        // Crée un AudioSource tout seul si le bouton n'en a pas
        source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f; // son 2D
    }

    private void OnEnable()  => button.onClick.AddListener(PlayClick);
    private void OnDisable() => button.onClick.RemoveListener(PlayClick);

    private void PlayClick() => Play(clickSound);

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable) Play(hoverSound);
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (button.interactable) Play(selectSound);
    }

    private void Play(AudioClip clip)
    {
        if (clip != null)
            source.PlayOneShot(clip, volume);
    }
}
