using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathSequence : MonoBehaviour
{
    public ScriptableObject deathVariable;   // asset "death"
    public AudioClip deathClip;
    [Range(0f, 1f)] public float deathVolume = 1f;
    [Range(0f, 1f)] public float otherSoundsVolume = 0.2f;  // musique de fond pendant la mort
    public GameObject player;                // vide = cherche le tag Player
    public Camera cam;                       // vide = Camera.main
    public float zoomDuration = 2f;
    public float zoomAmount = 0.5f;          // 0.5 = zoom à 50 % (plus petit = plus de zoom)
    public bool centerOnPlayer = true;
    public string sceneName = "game over_niv_2";

    bool started;
    bool dumped;
    const BindingFlags F = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    void Start()
    {
        if (!cam) cam = Camera.main;
        if (!player)
        {
            try { player = GameObject.FindWithTag("Player"); } catch { }
        }
        Time.timeScale = 1f;
        SetDeath(false);

        Debug.Log("DeathSequence prête. Variable: " + (deathVariable ? deathVariable.GetType().FullName : "AUCUNE") +
                  " | Player: " + (player ? player.name : "AUCUN"));
    }

    void Update()
    {
        if (!started && GetDeath())
        {
            started = true;
            StartCoroutine(Run());
        }
    }

    IEnumerator Run()
    {
        Debug.Log("Séquence de mort lancée");

        // Baisse les autres sons (musique de fond, etc.)
        foreach (var a in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
            a.volume *= otherSoundsVolume;

        // Son de mort en 2D, volume plein
        if (deathClip)
        {
            var go = new GameObject("DeathSound");
            var src = go.AddComponent<AudioSource>();
            src.clip = deathClip;
            src.spatialBlend = 0f;
            src.volume = deathVolume;
            src.ignoreListenerPause = true;
            src.Play();
            Destroy(go, deathClip.length + 0.5f);
        }

        // Freeze du joueur
        if (player)
        {
            foreach (var m in player.GetComponents<MonoBehaviour>())
                if (m != null) m.enabled = false;

            var rb = player.GetComponent<Rigidbody2D>();
            if (rb)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }

        // Coupe ce qui empêche le zoom (Cinemachine, Pixel Perfect Camera...)
        if (cam)
        {
            foreach (var b in cam.GetComponents<Behaviour>())
            {
                if (b == null || b == this) continue;
                string n = b.GetType().Name;
                if (n.Contains("CinemachineBrain") || n.Contains("PixelPerfectCamera"))
                {
                    b.enabled = false;
                    Debug.Log("Désactivé sur la caméra : " + n);
                }
            }
        }

        float startSize = cam ? cam.orthographicSize : 0f;
        float startFov = cam ? cam.fieldOfView : 0f;
        Vector3 startPos = cam ? cam.transform.position : Vector3.zero;
        Vector3 targetPos = startPos;
        if (cam && player && centerOnPlayer)
            targetPos = new Vector3(player.transform.position.x, player.transform.position.y, startPos.z);

        float t = 0f;
        while (t < zoomDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.SmoothStep(0f, 1f, t / zoomDuration);
            if (cam)
            {
                float k = Mathf.Lerp(1f, zoomAmount, p);
                if (cam.orthographic) cam.orthographicSize = startSize * k;
                else cam.fieldOfView = startFov * k;
                cam.transform.position = Vector3.Lerp(startPos, targetPos, p);
            }
            yield return null;
        }

        SetDeath(false);
        SceneManager.LoadScene(sceneName);
    }

    // ---------- Lecture / écriture de la variable ----------

    MemberInfo FindBool(out bool isField)
    {
        isField = false;
        if (!deathVariable) return null;

        for (var t = deathVariable.GetType(); t != null && t != typeof(ScriptableObject); t = t.BaseType)
        {
            foreach (var f in t.GetFields(F | BindingFlags.DeclaredOnly))
            {
                if (f.FieldType != typeof(bool)) continue;
                var n = f.Name.ToLower();
                if (n.Contains("initial") || n.Contains("persist")) continue;
                isField = true;
                return f;
            }
            foreach (var p in t.GetProperties(F | BindingFlags.DeclaredOnly))
            {
                if (p.PropertyType != typeof(bool) || !p.CanRead) continue;
                var n = p.Name.ToLower();
                if (n.Contains("initial") || n.Contains("persist")) continue;
                return p;
            }
        }
        return null;
    }

    void DumpMembers()
    {
        if (dumped || !deathVariable) return;
        dumped = true;
        string s = "Membres de " + deathVariable.GetType().FullName + " :\n";
        for (var t = deathVariable.GetType(); t != null && t != typeof(ScriptableObject); t = t.BaseType)
        {
            foreach (var f in t.GetFields(F | BindingFlags.DeclaredOnly)) s += "  champ " + f.FieldType.Name + " " + f.Name + "\n";
            foreach (var p in t.GetProperties(F | BindingFlags.DeclaredOnly)) s += "  propriété " + p.PropertyType.Name + " " + p.Name + "\n";
        }
        Debug.Log(s);
    }

    bool GetDeath()
    {
        var m = FindBool(out bool isField);
        if (m == null) { DumpMembers(); return false; }
        return isField ? (bool)((FieldInfo)m).GetValue(deathVariable)
                       : (bool)((PropertyInfo)m).GetValue(deathVariable);
    }

    void SetDeath(bool v)
    {
        var m = FindBool(out bool isField);
        if (m == null) return;
        if (isField) ((FieldInfo)m).SetValue(deathVariable, v);
        else if (((PropertyInfo)m).CanWrite) ((PropertyInfo)m).SetValue(deathVariable, v);
    }
}