using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ColliderEventSystem
{
    /// <summary>
    /// Loads or unloads a scene.
    /// </summary>
    public sealed class SceneAction : ActionBase
    {
        public enum Operation
        {
            Load,
            LoadAsync,
            Unload,
        }

        public Operation operation = Operation.Load;

#if UNITY_EDITOR
        [Tooltip("The scene to load or unload. Drag it in instead of typing its name - this also avoids " +
                 "the ambiguity Unity runs into when two scenes in the project share the same file name.")]
        public SceneAsset sceneAsset;
#endif

        [SerializeField, HideInInspector]
        private string scenePath;

        [Tooltip("If true, loads on top of the current scene(s) instead of replacing them. Only used by Load and Load Async - unloading has no additive equivalent.")]
        public bool additive;

        public override void Execute()
        {
            if (string.IsNullOrEmpty(scenePath)) return;

            if (operation == Operation.Unload)
            {
                SceneManager.UnloadSceneAsync(scenePath);
                return;
            }

            LoadSceneMode loadMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

            if (operation == Operation.LoadAsync)
            {
                SceneManager.LoadSceneAsync(scenePath, loadMode);
            }
            else
            {
                SceneManager.LoadScene(scenePath, loadMode);
            }
        }

#if UNITY_EDITOR
        // Resolves the SceneAsset to its asset path (e.g. "Assets/Scenes/Level1.unity") whenever it
        // changes in the Inspector. The full path - not just the name - is what actually gets serialized
        // and passed to SceneManager, so two scenes sharing a file name never collide.
        private void OnValidate()
        {
            scenePath = sceneAsset != null ? AssetDatabase.GetAssetPath(sceneAsset) : null;
        }

        public override bool TryGetValidationWarning(out string message)
        {
            if (sceneAsset == null)
            {
                message = null;
                return false;
            }

            foreach (EditorBuildSettingsScene buildScene in EditorBuildSettings.scenes)
            {
                if (buildScene.enabled && buildScene.path == scenePath)
                {
                    message = null;
                    return false;
                }
            }

            message = "This scene isn't enabled in Build Settings - it can't be loaded at runtime.";
            return true;
        }
#endif
    }
}
