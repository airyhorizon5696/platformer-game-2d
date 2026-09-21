using System.Collections.Generic;
using UnityEngine;

namespace ColliderEventSystem
{
    /// <summary>
    /// Shared temporary-material apply/restore, used by both Material Action's Restore Original mode and
    /// Condition's Hint Material. Sharing one cache means the two features never disagree about what "the
    /// original" was if they happen to touch the same Renderer.
    /// </summary>
    internal static class RendererMaterialOverride
    {
        private static readonly Dictionary<Renderer, Material[]> s_Originals = new Dictionary<Renderer, Material[]>();

#if UNITY_EDITOR
        // Renderers are recreated (or destroyed) between Play sessions, so cached entries from a
        // previous session are never valid to restore from - clear regardless of Fast Enter Play Mode /
        // domain reload settings.
        [UnityEditor.InitializeOnEnterPlayMode]
        private static void ResetCache()
        {
            s_Originals.Clear();
        }
#endif

        public static void Apply(Renderer renderer, Material material)
        {
            if (renderer == null || material == null) return;

            // Only remember the first Apply - a second Apply before a Restore (e.g. re-entering before
            // the exit fired) must not overwrite the real original with whatever's currently showing.
            if (!s_Originals.ContainsKey(renderer))
            {
                s_Originals[renderer] = renderer.sharedMaterials;
            }

            Material[] materials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++) materials[i] = material;
            renderer.materials = materials;
        }

        public static void Restore(Renderer renderer)
        {
            if (renderer == null) return;

            if (s_Originals.TryGetValue(renderer, out Material[] original))
            {
                renderer.materials = original;
                s_Originals.Remove(renderer);
            }
        }
    }
}
